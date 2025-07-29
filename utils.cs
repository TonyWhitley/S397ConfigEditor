using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace S397ConfigEditor;

public static class Utils
{
    /// <summary> Get the path of this source file </summary>
    public static string GetThisFilesPath(
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath =
            "") =>
        Directory.GetParent(sourceFilePath).ToString();

    /// <summary> Get the path of the exe file </summary>
    public static string GetThisExesPath() =>
        Directory.GetParent(Application.ExecutablePath).ToString();

}

/// <summary>
/// Class to handle text
/// </summary>
public static class TextUtils
{
    /// <summary>
    /// Put \n into 'text' if it goes over 'width' chars
    /// </summary>
    /// <param name="text">String to be wrapped</param>
    /// <param name="width">Max width of resulting text</param>
    public static string WrapText(string text, int width)
    {
        var originalWords = text.Split(new string[] { " " },
            StringSplitOptions.None);
        var result = new StringBuilder();
        var lineWidth = 0;

        foreach (var word in originalWords)
        {
            /*if (lineWidth != 0 && word.Contains("="))
            {
                result.Append("\n");
                lineWidth = 0;
            }*/
            result.Append(word + " ");
            lineWidth += word.Length + 1;

            if (lineWidth > width || word.EndsWith(","))
            {
                result.Append("\n");
                lineWidth = 0;
            }
        }

        return result.ToString();
    }
    public static string ToSentenceCase(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return str;
        str = str.Trim();
        return char.ToUpper(str[0]) + (str.Length > 1 ? str.Substring(1).ToLower() : "");
    }
}
