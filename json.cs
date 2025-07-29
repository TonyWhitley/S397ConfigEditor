using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json.Converters;

namespace S397ConfigEditor;

/// <summary>
/// Class to handle JSON files!
/// </summary>
public static class JsonFiles
{
    /// <summary>
    /// Read a JSON file, return a ContentDict
    /// </summary>
    public static ContentDict ReadJsonFile(string filepath) =>
        ReadJsonFile<ContentDict>(filepath);

    public static T ReadJsonFile<T>(string filepath)
    {
        string json;
        try
        {
            using var r = new StreamReader(filepath);
            json = r.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return default;
        }

        try
        {
            var obj = JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter()
                    }
                });
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return default;
        }
    }

    /// <summary>
    /// Create a ContentDict of dicts, one for each tab, from the filter file
    /// </summary>
    public static ContentDict ParseRF2PlayerEditorFilter(ContentDict rF2PlayerEditorFilter)
    {
        var tabs = new ContentDict();
        // Throw away the prepended Player.JSON key
        rF2PlayerEditorFilter.Remove("Player.JSON");
        foreach (var tabName in rF2PlayerEditorFilter)
        {
            if (!tabName.Key.Contains("#"))
            {
                //foreach (var f in rF2PlayerEditorFilter[tabName.Key].Children())
                tabs[tabName.Key] = rF2PlayerEditorFilter[tabName.Key]
                    .ToObject<ContentDict>();
            }
            // else it's a comment
        }

        return tabs;
    }

    /// <summary>
    /// Tweak JSON to rF2's JSON format
    /// </summary>
    /// <param name="JsonString"></param>
    /// <returns>rF2 format JSON string</returns>
    private static string JsonToRF2(string JsonString)
    {
        JsonString = JsonString.Replace("\": ", "\":");
        JsonString = JsonString.Replace("/", "\\/");
        JsonString = JsonString.Replace("Look Up\\/Down Angle",
            "Look Up/Down Angle");
        JsonString = JsonString.Replace("pixel\\/seconds", "pixel/seconds");
        return JsonString;
    }

    /// <summary>
    /// Write a ContentDict to JSON file.
    /// </summary>
    public static void WriteGameJsonFile(Game game, string filepath,
        ContentDict dictionary)
    {
        if (false) //game == Games.RF2)
        { // Now not sure what this does but LMU doesn't need it
            // Neither does rF2 it seems
            foreach (var section in dictionary)
            {
                foreach (var entry in dictionary[section.Key])
                {
                    if (entry.Name.Contains(" Version"))
                    {
                        // Version entries are strings not doubles "
                        dictionary[section.Key][entry.Name] = entry.Value;
                    }
                    else
                    {
                        dictionary[section.Key][entry.Name] =
                            WriteDict.TextToObject(entry.Value.ToString());
                    }
                }
            }
        }
        WriteJsonFile<ContentDict>(game, filepath, dictionary);
    }

    public static void WriteJsonFile<T>(Game game, string filepath, T obj)
    {
        var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented,
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            });
        if (game == Game.RF2)
        {
            jsonString = JsonToRF2(jsonString);
        }
        File.WriteAllText(filepath, jsonString);
    }

    /// <summary>
    /// Serialize object into a JSON string (unit test)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>JSON string</returns>
    public static string SerializeObject(object obj)
    {
        var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
        jsonString = JsonToRF2(jsonString);
        return jsonString;
    }
}
