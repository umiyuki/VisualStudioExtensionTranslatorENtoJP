using System.Collections.Generic;

namespace VSTranslator.Translation
{
	public class TranslationResult
	{
		public List<string> Sentences { get; set; }
		public List<DictionaryItem> DictionaryItems { get; set; }
		public string SourceLanguage { get; set; }
		public string DestinationLanguage { get; set; }

		public TranslationResult()
		{
			Sentences = new List<string>();
			DictionaryItems = new List<DictionaryItem>();
		}		
	}
}
