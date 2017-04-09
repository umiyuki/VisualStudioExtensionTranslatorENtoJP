using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTranslator.Translation
{
	public class DictionaryItem
	{
		public string Title { get; set; }
		public List<string> Terms { get; set; }

		public DictionaryItem()
		{
			Terms = new List<string>();
		}
	}

	
}
