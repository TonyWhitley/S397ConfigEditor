using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using CommandLine;

namespace S397ConfigEditor;

public class Config
{
    public string playerPath;

    public string playerJson;

    public string playerJsonPath;

    public string playerJsonFilter;

    public string playerEditorFilterJson;

    internal Config()
    {
        playerPath =
            @"c:\Program Files (x86)\Steam\steamapps\common\rFactor 2\UserData\player";
        playerJson = @"player.JSON";
        playerJsonPath =
            System.IO.Path.Combine(playerPath, playerJson);
        playerJsonFilter = @"rF2PlayerEditorFilter.JSON";

        playerEditorFilterJson =
            System.IO.Path.Combine(GetTheDataFilePath(),
                playerJsonFilter);
    }

    /// <summary> Get the path of this source file </summary>
    internal static string GetThisFilesPath(
        [System.Runtime.CompilerServices.CallerFilePath]
        string sourceFilePath = "") =>
        System.IO.Directory.GetParent(sourceFilePath).ToString();

    /// <summary> Get the path of the exe file </summary>
    internal static string GetThisExesPath() =>
        System.IO.Directory.GetParent(Application.ExecutablePath)
            .ToString();

    /// <summary> Get the path of the data file - the same as the exe
    /// if we're running as a program, the same as the source file if
    /// we're running under VS
    /// </summary>
    public string GetTheDataFilePath()
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            return GetThisFilesPath();
        }
        else
        {
            return GetThisExesPath();
        }
    }
}

public enum Game
{
    RF2,
    LMU
}
internal class Configs : IEnumerable<KeyValuePair<Game, Config>>
{
    private readonly Dictionary<Game, Config> configs = new()
    {
        {
            Game.RF2,
            new Config
            {
                playerPath =
                    "c:\\Program Files (x86)\\Steam\\steamapps\\common\\rFactor 2\\UserData\\player",
                playerJson = "Player.JSON",
                playerJsonPath =
                    "c:\\Program Files (x86)\\Steam\\steamapps\\common\\rFactor 2\\UserData\\player\\Player.JSON",
                playerJsonFilter = "rF2PlayerEditorFilter.JSON",
                playerEditorFilterJson =
                    "rF2PlayerEditorFilter.JSON"
            }
        },
        {
            Game.LMU,
            new Config
            {
                playerPath =
                    "c:\\Program Files (x86)\\Steam\\steamapps\\common\\Le Mans Ultimate\\UserData\\player",
                playerJson = "Settings.JSON",
                playerJsonPath =
                    "c:\\Program Files (x86)\\Steam\\steamapps\\common\\Le Mans Ultimate\\UserData\\player\\Settings.JSON",
                playerJsonFilter = "LMUSettingsFilter.JSON",
                playerEditorFilterJson =
                    "LMUSettingsFilter.JSON"
            }
        }
    };

    public void Add(Game key, Config value) => configs.Add(key, value);

    public Config this[Game key]
    {
        get { return configs[key]; }
        set { configs[key] = value; }
    }

    public IEnumerator<KeyValuePair<Game, Config>> GetEnumerator() => configs.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}


/// <summary>
/// This is an editor for S397's Player.json file.
/// It uses a second file rF2PlayerEditorFilter.json which can be modified
/// to change
/// a) Which keys are displayed
/// b) What tooltips are shown for each key
/// The values of the displayed keys are fetched from Player.json and
/// any changed values are written back there.
/// </summary>
/// <remarks>
/// ...how it does it...
/// 1) Read the JSON files into dicts player and filter
/// 2) Load values from player into keys in filter
/// 3) Split filter into a dict for each tab in the display
///     CHAT
///     DRIVING AIDS
///     Graphic Options
///         Detail levels   these breakdowns should be in rF2PlayerEditorFilter.json
///         Angles and sizes
///         Views
///     Race Conditions
///         General
///         CRNT
///         CHAMP
///         GPRIX
///         MULTI
///         RPLAY
///     Sound Options
/// </remarks>
internal static class S397ConfigEditor
{
    private static Configs configs; // Default or loaded from the file specified
    public static Config config;
    private static Game game;
    private static bool scripting = false;
    private const string EditorJsonPath = "PlayerEditorConfig.JSON";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param args>The command line args.</param>
    [STAThread]
    private static void Main(string[] args)
    {
        (game, config) = ParseCommandLine(args);

        if (File.Exists(EditorJsonPath))
        {
            configs = JsonFiles.ReadJsonFile<Configs>(EditorJsonPath);
        }

        configs ??= new Configs();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var runEditor = true;
        while (runEditor)
        {
            config = game == Game.LMU ? configs[Game.LMU] : configs[Game.RF2];

            var playerOriginal = JsonFiles.ReadJsonFile(config.playerJsonPath);
            var playerFilter =
                JsonFiles.ReadJsonFile(config.playerEditorFilterJson);
            JsonFiles.CopyDict(ref playerOriginal, out WriteDict.writeDict);
            // Get Player.JSON path from the file then remove it from the dictionary
            config.playerJsonPath = playerFilter[config.playerJson];
            playerFilter.Remove(config.playerJson);

            var tabs = JsonFiles.ParseRF2PlayerEditorFilter(playerFilter);
            // Copy values from Player.JSON to the dict used to display entries
            JsonFiles.CopyAllValuesToFilter(ref playerOriginal, ref tabs);

            if (scripting)
            {
                break;
            }

            var form = new Form1(tabs, game);
            Application.Run(form);
            var diff =
                WriteDict.GetDictionaryDifference(WriteDict.writeDict,
                    playerOriginal);
            if (diff.Count > 0)
            { // Content has changed, offer to save / save as
                form.SaveChanges();
                JsonFiles.WriteJsonFile(Game.LMU, EditorJsonPath, configs); // TBD: 
            }
            runEditor = form.ExitCode;
            // switch games
            game = game == Game.RF2 ? Game.LMU : Game.RF2;
        }
    }


    private class Options
    {
        [Option('s', "script", Required = false, HelpText = "The path to the JSON file containing edits.")]
        public string Script { get; set; }
        [Option('j', "json", Required = false, HelpText = "The path to the JSON file to be edited.")]
        public string Json { get; set; }
        [Option('r', "rf2", Required = false, HelpText = "Edit the rFactor 2 JSON file.")]
        public string Rf2 { get; set; }
        [Option('l', "lmu", Required = false, HelpText = "Edit the Le Mans Ultimate JSON file.")]
        public string Lmu { get; set; }
    }

    private static (Game, Config) ParseCommandLine(string[] args)
    {
        var game = Game.RF2; ;
        var config = new Config();
        var parser = new Parser(settings =>
        {
            settings.CaseSensitive = false;
        });
        parser.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                if (!String.IsNullOrEmpty(o.Script))
                {
                    Console.WriteLine($"Script input enabled. Current Arguments: --script {o.Script}");
                    scripting = true;
                }
                if (!String.IsNullOrEmpty(o.Json))
                {
                    Console.WriteLine($"JSON file to be edited. Current Arguments: --json {o.Json}");
                    config.playerJson = o.Json;
                }
                if (!String.IsNullOrEmpty(o.Rf2))
                {
                    Console.WriteLine("Edit the rFactor 2 JSON file");
                    game = Game.RF2;
                }
                if (!String.IsNullOrEmpty(o.Lmu))
                {
                    Console.WriteLine("Edit the Le Mans Ultimate JSON file");
                    game = Game.LMU;
                }
            });
        return (game, config);
    }

    public static void SaveChanges() =>
        JsonFiles.WriteGameJsonFile(game, config.playerJsonPath,
            WriteDict.writeDict);
}
