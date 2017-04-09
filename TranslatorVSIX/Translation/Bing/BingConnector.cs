using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSTranslator.Translation.Bing
{
    public class BingConnector
    {
        private const string BaseUrl = "http://api.microsofttranslator.com/v2/ajax.svc/";
        //access token
        private const string TokenUrl = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private const string TokenClientID = "VsTranslator";
        private const string TokenClientSecret = "SVJTxigXb3ezDDm6ZG5hn/FC20YUbV37clW3zw8hLLE=";

        private DateTime TokenExpires;
        private string CurrentToken;

        public BingConnector()
        {            
        }

        public string GetCurrentToken()
        {
            if ((TokenExpires - DateTime.Now).TotalMinutes < 1) { 
                RenewToken();                
            }
            return CurrentToken;
        }

        private void RenewToken()
        {
            var data = new Dictionary<string, string> {
                { "grant_type", "client_credentials" },
                { "client_id", TokenClientID },
                { "client_secret", TokenClientSecret },
                { "scope", "http://api.microsofttranslator.com" }                
            };

            string response = Utils.GetHttpResponse(TokenUrl, Utils.CreateQuerystring(data));
            JObject jToken = JObject.Parse(response);
            CurrentToken = jToken["access_token"].Value<string>();
            TokenExpires = DateTime.Now.AddSeconds(int.Parse(jToken["expires_in"].Value<string>()));
        }

        private string GetData(string method, Dictionary<string, string> data = null)
        {
            data = data ?? new Dictionary<string, string>();            
            WebClient client = new WebClient();
            client.Headers["Authorization"] = "Bearer " + GetCurrentToken();
            return client.DownloadString(BaseUrl + method + "?" + Utils.CreateQuerystring(data));                        
        }
        
        public virtual string GetLanguageNames(string locale, IEnumerable<string> codes)
        {            
            return GetData("GetLanguageNames", new Dictionary<string, string>{
                { "locale", "en" },
                { "languageCodes", JsonConvert.SerializeObject(codes) }
            });            
        }

        public virtual string GetLanguagesForTranslate()
        {
            return GetData("GetLanguagesForTranslate");            
        }

        public virtual string GetTranslations(string text, string sourceLang, string destLang)
        {
            return GetData("GetTranslations", new Dictionary<string, string> {                
                {"text", text},
				{"from", sourceLang},
				{"to", destLang},
				{"maxTranslations", "20"}
            });            
        }
    }
}
