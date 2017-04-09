﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace VSTranslator.Translation
{
	public class GoogleTranslator : BaseTranslator
	{
		public GoogleTranslator()
		{
            TargetLanguages = new List<TranslationLanguage>
			{
				new TranslationLanguage ("af", "Afrikaans"),
				new TranslationLanguage ("sq", "Albanian"),
				new TranslationLanguage ("ar", "Arabic"),
				new TranslationLanguage ("be", "Belarusian"),
				new TranslationLanguage ("bg", "Bulgarian"),
				new TranslationLanguage ("ca", "Catalan"),
				new TranslationLanguage ("zh-CN", "Chinese (Simplified)"),           
				new TranslationLanguage ("zh-TW", "Chinese (Traditional)"),           
				new TranslationLanguage ("hr", "Croatian"),
				new TranslationLanguage ("cs", "Czech"),
				new TranslationLanguage ("da", "Danish"),
				new TranslationLanguage ("nl", "Dutch"),
				new TranslationLanguage ("en", "English"),
				new TranslationLanguage ("et", "Estonian"),
				new TranslationLanguage ("tl", "Filipino"),
				new TranslationLanguage ("fi", "Finnish"),
				new TranslationLanguage ("fr", "French"),
				new TranslationLanguage ("gl", "Galician"),
				new TranslationLanguage ("de", "German"),
				new TranslationLanguage ("el", "Greek"),
				new TranslationLanguage ("iw", "Hebrew"),
				new TranslationLanguage ("hi", "Hindi"),
				new TranslationLanguage ("hu", "Hungarian"),
				new TranslationLanguage ("is", "Icelandic"),
				new TranslationLanguage ("id", "Indonesian"),
				new TranslationLanguage ("ga", "Irish"),
				new TranslationLanguage ("it", "Italian"),
				new TranslationLanguage ("ja", "Japanese"),
				new TranslationLanguage ("ko", "Korean"),
				new TranslationLanguage ("lv", "Latvian"),
				new TranslationLanguage ("lt", "Lithuanian"),
				new TranslationLanguage ("mk", "Macedonian"),
				new TranslationLanguage ("ms", "Malay"),
				new TranslationLanguage ("mt", "Maltese"),
				new TranslationLanguage ("fa", "Persian"),
				new TranslationLanguage ("pl", "Polish"),
				new TranslationLanguage ("pt", "Portugese"),
				new TranslationLanguage ("ro", "Romanian"),
				new TranslationLanguage ("ru", "Russian"),
				new TranslationLanguage ("sr", "Serbian"),
				new TranslationLanguage ("sk", "Slovak"),
				new TranslationLanguage ("sl", "Slovenian"),
				new TranslationLanguage ("es", "Spanish"),
				new TranslationLanguage ("sw", "Swahili"),
				new TranslationLanguage ("sv", "Swedish"),
				new TranslationLanguage ("th", "Thai"),
				new TranslationLanguage ("tr", "Turkish"),
				new TranslationLanguage ("uk", "Ukranian"),
				new TranslationLanguage ("vi", "Vietnamese"),
				new TranslationLanguage ("cy", "Welsh"),
				new TranslationLanguage ("yi", "Yiddish")
			};

            SourceLanguages = new List<TranslationLanguage> { new TranslationLanguage("", "Auto-detect") };
			SourceLanguages.AddRange(TargetLanguages);
		}	

		public override string Name
		{
			get { return "Google"; }
		}

		public override string AccessibleName
		{
			get { return "Google Translate"; }
		}
        	

		public override TranslationResult GetTranslation(string text, string sourceLang, string destinationLang)
		{
			string baseUrl = "https://translate.googleapis.com/translate_a/single";			
			/* {
					"sentences":[{"trans":"текст","orig":"text","translit":"tekst","src_translit":""}],
					"dict":
					[
							{"pos":"существительное","terms":["текст","текстовый файл","оригинал","руководство","тема","подлинный текст","цитата из библии"]},
							{"pos":"глагол","terms":["пис `ать крупным почерком"]}
					],
					"src":"en",
					"server_time":12
				}
			* */
						
			string data = Utils.CreateQuerystring(new Dictionary<string, string>()
            {
                {"client","gtx"},
                //{"otf", "1"},
				//{"pc", "0"},
				{"sl", sourceLang},
				{"tl", destinationLang},
				//{"hl", destinationLang},
				{"q", text},
                {"dt", "t" },
            });
			
			string response = Utils.GetHttpResponse(baseUrl, data);	
			var json = JArray.Parse(response);
			TranslationResult res = ParseResponse(json);
			res.DestinationLanguage = destinationLang;
			return res;
		}

        private TranslationResult ParseResponse(JArray json)
        {
            TranslationResult res = new TranslationResult();
            List<string> sentenceTexts = new List<string>();
            foreach (var cjson in json)
            {
                foreach (var ccjson in cjson)
                {
                    sentenceTexts.Add(ccjson[0].Value<string>());
                }
            }

            string fullText = string.Join("", sentenceTexts);
            if (!string.IsNullOrWhiteSpace(fullText))
            {
                res.Sentences.Add(fullText);
            }

            res.SourceLanguage = "en";
            return res;
        }

        private	TranslationResult ParseResponse(JObject json)
		{
			TranslationResult res = new TranslationResult();

			JToken sentences = json["sentences"];
            List<string> sentenceTexts = new List<string>();
            foreach (JToken sent in sentences.Children()) {
                sentenceTexts.Add(sent.Value<string>("trans"));
            }
            string fullText = string.Join(" ", sentenceTexts);
            if (!string.IsNullOrWhiteSpace(fullText)) {
                res.Sentences.Add(fullText);
            }

			JToken dictionary = json["dict"];
			if (dictionary != null)
			{
				foreach (JToken dToken in dictionary.Children())
				{
					DictionaryItem d = new DictionaryItem();
					d.Title = dToken.Value<string>("pos");
					JToken terms = dToken["terms"];
					foreach (JToken term in terms.Children())
					{
						d.Terms.Add(term.Value<string>());
					}
					res.DictionaryItems.Add(d);
				}
			}

			res.SourceLanguage = json.Value<string>("src");
			return res;
		}		
	}
}