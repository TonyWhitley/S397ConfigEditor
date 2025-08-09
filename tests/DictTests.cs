using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using S397ConfigEditor;

//using Tests;

using static S397ConfigEditor.Dictionaries;

namespace Tests
{
    [TestClass]
    public class DictTests
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    var dict1 = new ContentDict()
        //{
        //    { "A", 1 },
        //    { "B", "Hello" },
        //    { "C", 3.14 }
        //};

        //    var dict2 = new ContentDict()
        //{
        //    { "A", 1 },
        //    { "B", "World" },
        //    { "D", DateTime.Now }
        //};

        //    var diff = Dictionaries.GetDictionaryDifference(dict1, dict2);
        //    Assert.AreEqual(3, diff.Count);
        //}

        public class Controls
        {
            [JsonProperty("Current Control File")]
            public string CurrentControlFile { get; set; }

            [JsonProperty("Freelook Whilst Driving")]
            public bool FreelookWhilstDriving { get; set; }

            [JsonProperty("Freelook Whilst Driving#")]
            public string _FreelookWhilstDriving { get; set; }

            [JsonProperty("UI Gamepad Mouse - Final Speed (pixel/seconds)")]
            public double UIGamepadMouseFinalSpeedpixelseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Final Speed (pixel/seconds)#")]
            public string _UIGamepadMouseFinalSpeedpixelseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Initial Speed (pixel/seconds)")]
            public double UIGamepadMouseInitialSpeedpixelseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Initial Speed (pixel/seconds)#")]
            public string _UIGamepadMouseInitialSpeedpixelseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Time before accelerating (seconds)")]
            public double UIGamepadMouseTimebeforeacceleratingseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Time before accelerating (seconds)#")]
            public string _UIGamepadMouseTimebeforeacceleratingseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Time to reach max speed (seconds)")]
            public double UIGamepadMouseTimetoreachmaxspeedseconds { get; set; }

            [JsonProperty("UI Gamepad Mouse - Time to reach max speed (seconds)#")]
            public string _UIGamepadMouseTimetoreachmaxspeedseconds { get; set; }
        }

        public class DRIVER
        {
            [JsonProperty("AI Controls Driver")]
            public int AIControlsDriver { get; set; }

            [JsonProperty("AI Controls Driver#")]
            public string _AIControlsDriver { get; set; }

            [JsonProperty("Birth Date")]
            public string BirthDate { get; set; }

            [JsonProperty("Driver Hotswap Delay")]
            public double DriverHotswapDelay { get; set; }

            [JsonProperty("Driver Hotswap Delay#")]
            public string _DriverHotswapDelay { get; set; }

            [JsonProperty("Game Exit When Missing")]
            public bool GameExitWhenMissing { get; set; }

            [JsonProperty("Game Exit When Missing#")]
            public string _GameExitWhenMissing { get; set; }
            public string Location { get; set; }
            public string Nationality { get; set; }

            [JsonProperty("Original Driver")]
            public string OriginalDriver { get; set; }

            [JsonProperty("Package Dir")]
            public string PackageDir { get; set; }

            [JsonProperty("Player Name")]
            public string PlayerName { get; set; }

            [JsonProperty("Player Nick")]
            public string PlayerNick { get; set; }
            public string Showroom { get; set; }

            [JsonProperty("Showroom Component")]
            public string ShowroomComponent { get; set; }

            [JsonProperty("Showroom Component Version")]
            public string ShowroomComponentVersion { get; set; }

            [JsonProperty("Starting Driver")]
            public int StartingDriver { get; set; }

            [JsonProperty("Starting Driver#")]
            public string _StartingDriver { get; set; }
            public string Team { get; set; }
            public string Vehicle { get; set; }
        }

        public class Root
        {
            public Controls Controls { get; set; }
            public DRIVER DRIVER { get; set; }
        }

        [Ignore]
        [TestMethod]
        public void TestMethod2()
        {
            Root testTree = new Root();
            var root = new TreeNode("root");
            var Controls = new ContentEntry();
            var DRIVER = new ContentEntry();

            Controls["CurrentControlFile"] = "path/to/file";
            Controls["FreelookWhilstDriving"] = true;
            DRIVER["AIControlsDriver"] = 1;
            DRIVER["PlayerName"] = "Test Driver";
            var xxx = new ContentDict();
            xxx["Controls"] = Controls;
            xxx["Driver"] = DRIVER;
            var tree = Dictionaries.FlattenTree(xxx);

            var yyy = Dictionaries.RebuildTree(tree);
            //var diff = Dictionaries.GetDictionaryDifference(xxx, yyy);
            Assert.AreEqual(xxx, yyy);



            //root.Children.Add(new TreeNode(testTree));
            //var paths = new List<(List<object>, object)>();
            //TreeUtils.CollectLeafPaths(root, new List<object>(), paths);
            //Assert.IsTrue(paths.Count > 0, "No paths collected from the tree node.");
            //var result = TreeUtils.RebuildTree(paths);
            //Assert.AreEqual(result, testTree); 
        }

        [TestMethod]
        public void TestRealData()
        {
            ContentDict player = JsonFilesTests.readrF2PlayerEditorFilterJson();
            var tree = Dictionaries.FlattenTree(player);

            var newTree = Dictionaries.RebuildTree(tree);
            var diffs = GetDictionaryDifference(player, newTree);
            Assert.AreEqual(diffs.Count, 0, "Rebuilt tree does not match the original player dictionary.");
        }

        //[TestMethod]
        //public void TestDemo()
        //{
        //    var root = new TreeNode("root");
        //    var a = new TreeNode(42);
        //    var b = new TreeNode(true);
        //    var c = new TreeNode("leaf1");
        //    var d = new TreeNode(3.14);
        //    var e = new TreeNode(false);

        //    root.Children.Add(a);
        //    root.Children.Add(b);
        //    a.Children.Add(c);  // testTree -> 42 -> "leaf1"
        //    b.Children.Add(d);  // testTree -> true -> 3.14
        //    d.Children.Add(e);  // testTree -> true -> 3.14 -> false

        //    // Extract leaf paths
        //    var paths = new List<(List<object>, object)>();
        //    TreeUtils.CollectLeafPaths(root, new List<object>(), paths);

        //    Console.WriteLine("Leaf Paths:");
        //    foreach (var (path, _) in paths)
        //        Console.WriteLine(string.Join(" -> ", path));

        //    // Rebuild tree
        //    var rebuilt = TreeUtils.RebuildTree(paths);

        //    Console.WriteLine("\nReconstructed Tree:");
        //    TreeUtils.PrintTree(rebuilt);
        //}
    }
}
