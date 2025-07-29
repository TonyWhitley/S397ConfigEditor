using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using S397ConfigEditor;

namespace Tests
{
    [TestClass()]
    public class JsonFilesTests
    {
        private readonly string playerJson =
            @"c:\Program Files (x86)\Steam\steamapps\common\rFactor 2\UserData\player\player.JSON";

        private readonly string rF2PlayerEditorFilterJson =
            "../../rF2PlayerEditorFilter.JSON";

        /// <summary>Read Player.JSON</summary>
        [TestMethod()]
        public void ReadPlayerJsonFileTest()
        {
            var player = JsonFiles.ReadJsonFile(playerJson);
            Assert.IsNotNull(player["CHAT"]);
        }

        ///<summary>Read the config file</summary>
        [TestMethod()]
        public void ReadrF2PlayerEditorFilterJsonFileTest()
        {
            var player = JsonFiles.ReadJsonFile(rF2PlayerEditorFilterJson);
            Assert.IsNotNull(player["Chat"]);
        }

        ///<summary>
        ///Text to JSON test
        ///Convert text back to an object, serialize the object, check that the
        ///format is as expected
        ///</summary>
        [TestMethod()]
        public void TextToJsonTest()
        {
            string key;
            string value;
            string testString;
            object JsonObj;
            var testDict = new Dictionary<string, dynamic>();

            key = "Quick Chat #10";
            value = "true";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual(true, JsonObj);
            testDict[key] = JsonObj;
            testString = JsonFiles.SerializeObject(testDict);
            Assert.AreEqual("{\r\n  \"Quick Chat #10\":true\r\n}", testString);
            testDict.Remove(key);

            key = "Antilock Brakes";
            value = "0";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual((long) 0, JsonObj);
            testDict[key] = JsonObj;
            testString = JsonFiles.SerializeObject(testDict);
            Assert.AreEqual("{\r\n  \"Antilock Brakes\":0\r\n}", testString);
            testDict.Remove(key);

            value = "Some text";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual(value, JsonObj);

            key = "Quick Chat #10";
            value = "/vote yes";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual(value, JsonObj);
            testDict[key] = JsonObj;
            testString = JsonFiles.SerializeObject(testDict);
            Assert.AreEqual("{\r\n  \"Quick Chat #10\":\"\\/vote yes\"\r\n}",
                testString);
            testDict.Remove(key);

            key = "Look Up/Down Angle";
            value = "0";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual((long) 0, JsonObj);
            testDict[key] = JsonObj;
            testString = JsonFiles.SerializeObject(testDict);
            Assert.AreEqual("{\r\n  \"Look Up/Down Angle\":0\r\n}", testString);
            testDict.Remove(key);


            key = "pixel/seconds";
            value = "550";
            JsonObj = WriteDict.TextToObject(value);
            Assert.AreEqual((long) 550, JsonObj);
            testDict[key] = JsonObj;
            testString = JsonFiles.SerializeObject(testDict);
            Assert.AreEqual("{\r\n  \"pixel/seconds\":550\r\n}", testString);
            testDict.Remove(key);
        }
    }

    [TestClass()]
    public class DictTests
    {
        /// <summary> Read the filter JSON file and split it </summary>
        private Dictionary<string, dynamic> GetRF2PlayerEditorFilterTabsFromJsonFile()
        {
            var playerFilter =
                JsonFiles.ReadJsonFile(rF2PlayerEditorFilterJson);
            var tabs = JsonFiles.ParseRF2PlayerEditorFilter(playerFilter);
            return tabs;
        }

        private readonly string playerJson =
            @"c:\Program Files (x86)\Steam\steamapps\common\rFactor 2\UserData\player\player.JSON";

        private readonly string rF2PlayerEditorFilterJson =
            "../../rF2PlayerEditorFilter.JSON";

        /// <summary> Read the filter JSON file and split it. Check the first tab exists </summary>
        [TestMethod()]
        public void SplitRF2PlayerEditorFilterJsonFileTest()
        {
            var tabs = GetRF2PlayerEditorFilterTabsFromJsonFile();
            Assert.IsNotNull(tabs["Chat"]);
        }

        /// <summary> Split the filter file into tabs and check the contents of the first tab </summary>
        [TestMethod()]
        public void SplitTabsTest()
        {
            var tabs = GetRF2PlayerEditorFilterTabsFromJsonFile();
            Assert.IsNotNull(tabs["Chat"]);

            Dictionary<string, dynamic> tab = tabs["Chat"];
            Assert.IsNotNull(tab["CHAT"]);
        }

        /// <summary> Copy the values from one ContentDict into the same keys of another </summary>
        [TestMethod()]
        public void DictCopyTest()
        {
            var player = JsonFiles.ReadJsonFile(playerJson);
            var tabs = GetRF2PlayerEditorFilterTabsFromJsonFile();
            Assert.IsNotNull(tabs["Chat"]);
            tabs["Chat"]["CHAT"]["Quick Chat #1"] = ""; // Delete the string
            Dictionary<string, dynamic> tabChat = tabs["Chat"];
            Dictionaries.CopyDictValues(ref player, ref tabChat);
            string res = tabChat["CHAT"]["Quick Chat #1"];
            Assert.AreEqual("Slowing to pit", res);
        }

        /// <summary> Copy the values from Player.JSON into the same keys of
        /// of rF2PlayerEditorFilter.JSON </summary>
        [TestMethod()]
        public void JsonCopyTest()
        {
            var player = JsonFiles.ReadJsonFile(playerJson);
            var tabs = GetRF2PlayerEditorFilterTabsFromJsonFile();
            Assert.IsNotNull(tabs["Chat"]);
            string res = tabs["Chat"]["CHAT"]["Quick Chat #1"];
            Assert.AreEqual("", res);
            Dictionaries.CopyAllValuesToFilter(ref player, ref tabs);
            res = tabs["Chat"]["CHAT"]["Quick Chat #1"];
            Assert.AreEqual("Slowing to pit", res);
        }

        [TestMethod()]
        public void WriteFailTest()
        {
            var tabs = JsonFiles.ReadJsonFile(playerJson);
            WriteDict.EditedContent = tabs;
            var ret = WriteDict.WriteValue("HELLO", "WORLD");
            Assert.IsFalse(ret);
        }

        [TestMethod()]
        public void WriteSucceedTest()
        {
            var tabs = JsonFiles.ReadJsonFile(playerJson);
            WriteDict.EditedContent = tabs;
            var ret = WriteDict.WriteValue("Quick Chat #1", "Please pass");
            Assert.IsTrue(ret);
        }
    }

    [TestClass()]
    public class StringTests
    {
        [TestMethod()]
        public void WrapTest()
        {
            var input =
                "Details and visible vehicles will be automatically reduced (by up to half) if framerate is under this threshold (0 to disable)";
            var wrapped = TextUtils.WrapText(input, 40);
            Assert.AreEqual(
                "Details and visible vehicles will be automatically \nreduced (by up to half) if framerate is under \nthis threshold (0 to disable) ",
                wrapped);
        }
        [TestMethod()]
        public void CommaTest()
        {
            var input =
                "0 = no repeat shift detection, 1 = detect and eliminate accidental repeat shifts within 100ms, 2 = 150ms, 3 = 200ms, 4 = 250ms, 5 = prevent shifting again before previous shift is completed";
            var wrapped = TextUtils.WrapText(input, 40);
            Assert.AreEqual(
                "0 = no repeat shift detection, \n1 = detect and eliminate accidental repeat \nshifts within 100ms, \n2 = 150ms, \n3 = 200ms, \n4 = 250ms, \n5 = prevent shifting again before previous \nshift is completed ",
                wrapped);
        }
        [TestMethod()]
        public void CommaTest2()
        {
            var input =
                "0=Off, 1=Player Only, 2=Everybody";
            var wrapped = TextUtils.WrapText(input, 40);
            Assert.AreEqual(
                "0=Off, \n1=Player Only, \n2=Everybody ",
                wrapped);
        }
    }
}
