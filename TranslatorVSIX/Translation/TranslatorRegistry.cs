using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSTranslator.Translation.Bing;

namespace VSTranslator.Translation
{
	public class TranslatorRegistry
	{
		public static List<BaseTranslator> Translators = new List<BaseTranslator> {
			new GoogleTranslator(),
            new BingTranslator(new BingConnector())            
		};

		public static BaseTranslator GetTranslator(string name)
		{
			return Translators.Find(t => t.Name == name);
		}
	}
}
