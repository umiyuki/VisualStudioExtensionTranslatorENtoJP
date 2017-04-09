using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTranslator.Translation
{
	public class TranslationLanguage
	{
		public string Code { get; set; }
		public string Name { get; set; }

		public TranslationLanguage(string code, string name)
		{
			Code = code;
			Name = name;
		}
	}
}
