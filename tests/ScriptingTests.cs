using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ScriptingTests
    {
        private ContentEntry changes = new ContentEntry()
        {
            { "Antilock Brakes", 0 }
        };

        private string changesJson =
            "{\r\n  \"Player.JSON\" : \"c:\\\\Program Files (x86)\\\\Steam\\\\steamapps\\\\common\\\\rFactor 2\\\\UserData\\\\player\\\\player.Json\",\r\n  \"#Created from player.JSON\" : \"Grouped into Tab names, reusing the original section within that\",\r\n  \"DRIVING AIDS\": {\r\n    \"Antilock Brakes\": 0\r\n    },\r\n  \"Graphic Options\": {\r\n    \"HUD\": 6\r\n    }\r\n}";

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
