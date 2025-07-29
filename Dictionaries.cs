using System.Collections.Generic;

using Newtonsoft.Json;

namespace S397ConfigEditor;
public class Dictionaries
{

    /// <summary>
    /// Copy the values from a ContentDict to a another ContentDict
    /// </summary>
    public static void CopyDictValues(ref ContentDict fromContentDict, ref ContentDict toContentDict)
    {
        foreach (var entry in toContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
        {
            var name = entry.Key;
            foreach (var key in toContentDict[name])
            {
                if (!key.Name.EndsWith("#") && // If it's not a comment
                    fromContentDict[name][key.Name] !=
                    null) // and from_dict has the key
                {
                    toContentDict[name][key.Name] = fromContentDict[name][key.Name];
                }
            }
        }
    }

    /// <summary>
    /// Copy the values from a ContentDict to this program's filter ContentDict
    /// </summary>
    public static void CopyAllValuesToFilter(ref ContentDict fromContentDict, ref ContentDict toContentDict)
    {
        foreach (var entry in toContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
        {
            ContentDict tabContentDict = toContentDict[entry.Key];
            CopyDictValues(ref fromContentDict, ref tabContentDict);
        }
    }

    /// <summary>
    /// Copy the values from this program's filter ContentDict to a another ContentDict
    /// </summary>
    public static void CopyAllValuesFromFilter(ref ContentDict fromContentDict,
        ref ContentDict toContentDict)
    {
        foreach (var entry in
                 fromContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
        {
            ContentDict tabContentDict = fromContentDict[entry.Key];
            CopyDictValues(ref tabContentDict, ref toContentDict);
        }
    }

    /// <summary>
    /// Copy the values from a tab's ContentDict to the Player ContentDict
    /// </summary>
    public static void CopyAllValuesFromTab(ref ContentDict fromContentDict, ref ContentDict toContentDict)
    {
        foreach (var entry in
                 fromContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
        {
            ContentDict tabContentDict = fromContentDict[entry.Key];
            CopyDictValues(ref tabContentDict, ref toContentDict);
        }
    }

    /// <summary>
    /// Copy a ContentDict to a another ContentDict
    /// </summary>
    public static void CopyDict(ref ContentDict fromContentDict, out ContentDict toContentDict) =>
        // Deep copy using JSON serialization
        toContentDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
            JsonConvert.SerializeObject(fromContentDict));
}


public static class WriteDict
{
    /// <summary>
    /// The dictionary that has the edited content that will be written
    /// to the selected output JSON file
    /// </summary>
    public static ContentDict EditedContent;
    public static bool changed = false;

    /// <summary>
    /// Translate a string to an object
    /// </summary>
    /// <param name="value">int, bool, float, string</param>
    /// <returns></returns>
    public static object TextToObject(string value)
    {
        if (bool.TryParse(value, out var boolResult))
        {
            return boolResult;
        }

        if (long.TryParse(value, out var longResult))
        {
            return longResult;
        }

        if (double.TryParse(value, out var doubleResult))
        {
            return doubleResult;
        }

        if (float.TryParse(value, out var floatResult))
        {
            return floatResult;
        }

        return value;
    }

    /// <summary>
    /// Write value to the key found somewhere in the ContentDict
    /// </summary>
    /// <param name="key">key name</param>
    /// <param name="value">the text to be written</param>
    /// <returns></returns>
    public static bool WriteValue(string key, string value)
    {
        foreach (var tabData in EditedContent) // HACKERY!!!
        {
            var group = tabData.Key;
            ContentDict contentDict = tabData.Value.ToObject<ContentDict>();
            if (contentDict.ContainsKey(key))
            {
                EditedContent[group][key] = value;
                changed = true;
                return true;
            }
        }

        return false; // didn't find the key
    }

    /// <summary>
    /// We have two dictionaries, dict1 and dict2, both of type <string, dynamic>. 
    /// The GetDictionaryDifference method takes these dictionaries as 
    /// input and returns a new dictionary, diff, that contains the differences
    /// between the two dictionaries.

    /// The AreValuesEqual method is introduced to perform dynamic comparison
    /// of the values. It uses the equality operator (==) to compare the 
    /// values.
    /// </summary>
    /// <param name="dict1"></param>
    /// <param name="dict2"></param>
    /// <returns>the differences between the two dictionaries.</returns>
    public static ContentDict GetDictionaryDifference(ContentDict dict1, ContentDict dict2)
    {
        // Create a new dictionary to store the differences
        var diff = new ContentDict();

        // Iterate over the keys in dict1
        foreach (var kvp in dict1)
        {
            var key = kvp.Key;
            var value1 = kvp.Value;

            // Check if dict2 contains the key
            if (dict2.TryGetValue(key, out var value2))
            {
                // The key exists in both dictionaries, compare the values
                if (!AreValuesEqual(value1, value2))
                {
                    // Values are different, add to the diff dictionary
                    diff.Add(key, value2);
                }
            }
            else
            {
                // The key doesn't exist in dict2, add to the diff dictionary
                diff.Add(key, value1);
            }
        }

        // Check for keys in dict2 that don't exist in dict1
        foreach (var kvp in dict2)
        {
            var key = kvp.Key;

            if (!dict1.ContainsKey(key))
            {
                // The key doesn't exist in dict1, add to the diff dictionary
                diff.Add(key, kvp.Value);
            }
        }

        return diff;
    }

    static bool AreValuesEqual(dynamic value1, dynamic value2)
    {
        // JObject/JToken deep comparison
        if (value1 is Newtonsoft.Json.Linq.JToken token1 && value2 is Newtonsoft.Json.Linq.JToken token2)
            return Newtonsoft.Json.Linq.JToken.DeepEquals(token1, token2);
        // Compare the values using dynamic comparison
        return value1 == value2;
    }
}

