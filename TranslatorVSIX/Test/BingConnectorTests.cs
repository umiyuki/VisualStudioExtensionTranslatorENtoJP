/*using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VSTranslator.Translation.Bing;

namespace Translator.Tests
{
    [TestClass]
    public class BingConnectorTests
    {
        [TestMethod]
        public void GetToken()
        {
            BingConnector conn = new BingConnector();
            string t1 = conn.GetCurrentToken();
            //token should not be renewed between calls
            string t2 = conn.GetCurrentToken();
            Assert.IsTrue(t1 == t2);
        }

        [TestMethod]
        public void GetLanguageNames()
        {
            BingConnector conn = new BingConnector();
            string codesResponse = conn.GetLanguagesForTranslate();            
            List<string> codes = JsonConvert.DeserializeObject<List<string>>(codesResponse);
            Assert.IsTrue(codes.Count != 0);

            string namesResponse = new BingConnector().GetLanguageNames("en", codes);
            List<string> names = JsonConvert.DeserializeObject<List<string>>(namesResponse);
            Assert.IsTrue(names.Count != 0);
        }

        [TestMethod]
        public void GetTranslations()
        {
            BingConnector conn = new BingConnector();
            string response = conn.GetTranslations("go", "en", "ru");
            JObject jObj = JObject.Parse(response);
            Assert.IsTrue(jObj["From"] != null && jObj["Translations"] != null);
        }

    }
}
*/