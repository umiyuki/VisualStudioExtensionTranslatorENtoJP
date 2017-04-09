/*using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VSTranslator.Translation;
using VSTranslator.Translation.Bing;

namespace Translator.Tests
{
    [TestClass]
    public class TranslatorTests
    {
        [TestMethod]
        public void TranslationTest()
        {
            var mock = new Mock<BingConnector>();
            //response contains duplicates and is not sorted by rating or match degree
            mock.Setup(c => c.GetTranslations("go", "en", "ru"))
                .Returns(@"{
                    From:""en"",
                    Translations:[
                        {Count:0,MatchDegree:100,MatchedOriginalText:""go"",Rating:0,TranslatedText:""ПРОДОЛЖАТЬ""},
                        {Count:0,MatchDegree:100,MatchedOriginalText:"""",Rating:0,TranslatedText:""перейти""},
                        {Count:0,MatchDegree:100,MatchedOriginalText:"""",Rating:5,TranslatedText:""перейти""} 
                    ]
                }");

            BingConnector conn = mock.Object;

            TranslationResult tr = new BingTranslator(conn).GetTranslation("go", "en", "ru");
            Assert.IsTrue(tr.Sentences.Count == 2 
                            && tr.Sentences[0] == "перейти"
                            && tr.Sentences[1] == "ПРОДОЛЖАТЬ"
                            && tr.SourceLanguage == "en"
                            && tr.DestinationLanguage == "ru",
                            "Unexpected result"
                        );                          
        }


        [TestMethod]
        public void LanguageTest()
        {
            var mock = new Mock<BingConnector>();
            mock.Setup(c => c.GetLanguagesForTranslate())
                .Returns("[\"ru\", \"en\"]");
            mock.Setup(c => c.GetLanguageNames("en", new List<string> { "ru", "en" }))
                .Returns("[\"Russian\", \"English\"]");

            List<TranslationLanguage> langs = new BingTranslator(mock.Object).TargetLanguages;
                       
            Assert.IsTrue(langs != null                
                            && langs.Count == 2
                            && langs[0].Code == "en"
                            && langs[0].Name == "English"
                            && langs[1].Code == "ru"
                            && langs[1].Name == "Russian"                            
                        );
        }
    }
}*/
