using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTranslator.Translation
{
	public abstract class BaseTranslator
	{
		public abstract TranslationResult GetTranslation(string text, string sourceLang, string destinationLang);
        public virtual List<TranslationLanguage> SourceLanguages { get; protected set; }
        public virtual List<TranslationLanguage> TargetLanguages { get; protected set; }
		public abstract string AccessibleName { get; }
		public abstract string Name { get; }

		public bool CanSwap(string sourceLang, string targetLang)
		{
			return TargetLanguages.Find(l => l.Code == sourceLang) != null
				&& SourceLanguages.Find(l => l.Code == targetLang) != null;
		}
	}
}
