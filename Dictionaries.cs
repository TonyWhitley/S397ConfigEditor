using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace S397ConfigEditor;
public class Filter
{
    public Dictionary<string, string>S397ConfigEditor;
    public Dictionary<string, ContentEntry> Tabs;
    public Filter()
    {
        this.Tabs = new();
    }
}
public class Dictionaries
{
    /// <summary>
    /// Copy the values from a ContentDict to a another ContentDict
    /// </summary>
    public static void CopyDictValues(in ContentDict fromContentDict, ref ContentDict toContentDict)
    {
        foreach (var entry in toContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
        {
            var name = entry.Key;
            foreach (var key in toContentDict[name])
            {
                if (!key.Key.EndsWith("#") && // If it's not a comment
                    fromContentDict[name][key.Key] !=
                    null) // and from_dict has the key
                {
                    toContentDict[name][key.Key] = fromContentDict[name][key.Key];
                }
            }
        }
    }

    /// <summary>
    /// Copy the values from a ContentDict to this program's filter ContentDict
    /// </summary>
    public static void CopyAllValuesToFilter(in ContentDict fromContentDict, ref Filter toContentFilter)
    {
        var toContentFilterTabs = JsonConvert.DeserializeObject<ContentDict>(JsonConvert.SerializeObject(toContentFilter.Tabs));
        foreach (var entry in toContentFilterTabs)
        {
            var tabContentDict = entry.Value;
            var contentUsedInTab = tabContentDict.Keys.First();
            var allContentTabMightUse = fromContentDict[contentUsedInTab];
            foreach (var entry2 in toContentFilterTabs)
            {
                var name = entry2.Key;
                
                foreach (var entry3 in toContentFilterTabs[name])
                {
                    foreach (var key in entry3.Value)
                    {
                        if (!key.Name.EndsWith("#") && // If it's not a comment
                            fromContentDict[entry3.Key] !=
                            null) // and from_dict has the key
                        {
                            try
                            {
                                toContentFilter.Tabs[name][key.Name] = fromContentDict[entry3.Key][key.Name];
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Game JSON doesn't have [{entry3.Key}][{key.Name}]");
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Copy the values from this program's filter ContentDict to another ContentDict
    /// </summary>
    //public static void CopyAllValuesFromFilter(in ContentDict fromContentDict, ref ContentDict toContentDict)
    //{
    //    foreach (var entry in
    //             fromContentDict) // to_dict[to_dict.Keys].ToObject<ContentDict>())
    //    {
    //        ContentEntry tabContentEntry = entry.Value;
    //        //CopyDictValues(tabContentEntry, ref toContentDict);
    //    }
    //}

    /// <summary>
    /// Copy the values from a tab's ContentEntry to the Player ContentDict
    /// </summary>
    //public static void CopyAllValuesFromTab(in ContentEntry allContentTabMightUse, ref ContentDict toContentDict)
    //{
    //    foreach (var entry in
    //             allContentTabMightUse) // to_dict[to_dict.Keys].ToObject<ContentDict>())
    //    {
    //        ContentDict tabContentDict = allContentTabMightUse[entry.Key];
    //        CopyDictValues(tabContentDict, ref toContentDict);
    //    }
    //}

    /// <summary>
    /// Copy a ContentDict to a another ContentDict
    /// </summary>
    public static void CopyDict(in ContentDict fromContentDict, out ContentDict toContentDict) =>
        // Deep copy using JSON serialization
        toContentDict = JsonConvert.DeserializeObject<ContentDict>(
            JsonConvert.SerializeObject(fromContentDict));

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
    public static ContentDict GetDictionaryDifference(in ContentDict dict1, in ContentDict dict2)
    {
        var diff = new ContentDict();

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
                    // TBD need to drill down into JToken objects and compare them
                    foreach (var kvp2 in value1)
                    {
                        var subKey = kvp2.Key;
                        //((Newtonsoft.Json.Linq.JProperty)kvp2).Name;
                        var subValue1 = kvp2.Value;
                        if (value2.ContainsKey(subKey))
                        {
                            var subValue2 = value2[subKey];
                            if (!AreValuesEqual(subValue1, subValue2))
                            {
                                // Values are different, add to the diff dictionary
                                diff.Add(key, kvp2.Value);
                            }
                        }
                        else
                        {
                            // The key doesn't exist in dict2, add to the diff dictionary
                            diff.Add($"{key}.{subKey}", subValue1);
                        }
                    }
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

    public static List<(string Group, string Name, object Value)> FlattenTree(ContentDict tree)
    {
        var result = new List<(string, string, object)>();

        foreach (var groupEntry in tree)
        {
            var group = groupEntry.Key;
            var items = groupEntry.Value;

            foreach (var itemEntry in items)
            {
                var name = itemEntry.Key;
                var value = itemEntry.Value;

                result.Add((group, name, value));
            }
        }

        return result;
    }

    public static ContentDict RebuildTree(List<(string Group, string Name, object Value)> flatList)
    {
        var tree = new ContentDict();

        foreach (var entry in flatList)
        {
            var group = entry.Group;
            var name = entry.Name;
            var value = entry.Value;

            if (!tree.ContainsKey(group))
                tree[group] = new Dictionary<string, object>();

            tree[group][name] = value;
        }

        return tree;
    }



    public class TreeNode
    {
        public object Value;
        public List<TreeNode> Children = new List<TreeNode>();

        public TreeNode(object value)
        {
            Value = value;
        }
    }
    public class TreeUtils
    {
        // Step 1: Traverse and collect leaf paths
        public static void CollectLeafPaths(TreeNode node, List<object> path, List<(List<object> Path, object LeafValue)> result)
        {
            path.Add(node.Value);

            if (node.Children.Count == 0)
            {
                result.Add((new List<object>(path), node.Value));
            }
            else
            {
                foreach (var child in node.Children)
                    CollectLeafPaths(child, path, result);
            }

            path.RemoveAt(path.Count - 1);
        }

        // Step 2: Rebuild tree from leaf paths
        public static TreeNode RebuildTree(List<(List<object> Path, object LeafValue)> leafPaths)
        {
            if (leafPaths.Count == 0) return null;

            var root = new TreeNode(leafPaths[0].Path[0]);

            foreach (var (path, _) in leafPaths)
            {
                TreeNode current = root;

                for (int i = 1; i < path.Count; i++)
                {
                    var existing = current.Children.Find(c => Equals(c.Value, path[i]));
                    if (existing == null)
                    {
                        existing = new TreeNode(path[i]);
                        current.Children.Add(existing);
                    }

                    current = existing;
                }
            }

            return root;
        }

        // Helper: Print tree
        public static void PrintTree(TreeNode node, string indent = "")
        {
            Console.WriteLine(indent + node.Value);
            foreach (var child in node.Children)
                PrintTree(child, indent + "  ");
        }
    }

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
            ContentEntry contentDict = tabData.Value;
            if (contentDict.ContainsKey(key))
            {
                EditedContent[group][key] = value;
                changed = true;
                return true;
            }
        }

        return false; // didn't find the key
    }
}




