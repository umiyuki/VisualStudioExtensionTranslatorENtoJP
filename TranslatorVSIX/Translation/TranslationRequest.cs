using System;
using System.Threading;
using System.Text.RegularExpressions;

namespace VSTranslator.Translation
{
	public class TranslationRequest
	{
		public string Text { get; set; }
		public BaseTranslator Translator { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
		public TranslationResult Result { get; set; }
		public Exception Exception { get; set; }

		public TranslationRequest(string text, BaseTranslator translator, string sourceLanguage, string targetLanguage)
		{
			Text = text;
			Translator = translator;
			SourceLanguage = sourceLanguage;
			TargetLanguage = targetLanguage;
		}

		public void GetTranslationAsync()
		{
			new Thread(TranslationThread).Start();			
		}

		private void TranslationThread()
		{
			Result = null;
			Exception = null;			
			try
			{
				Result = Translator.GetTranslation(SplitIdentifiers(Text), SourceLanguage, TargetLanguage);				
			}
			catch (Exception e)
			{
				Exception = e;
			}
			OnTranslationComplete();
		}

		public event EventHandler TranslationComplete;
		private void OnTranslationComplete()
		{
			if (TranslationComplete != null)
				TranslationComplete(this, EventArgs.Empty);
		}

		private static Regex CamelCaseRegex = new Regex(@"\p{Ll}\p{Lu}", RegexOptions.Compiled | RegexOptions.Multiline);

		/// <summary>
		/// splits prefix_camelCase -> prefix camel case
		/// </summary>		
		private static string SplitIdentifiers(string text)
		{
			return CamelCaseRegex.Replace(text.Replace("_", " "), m => m.Value[0] + " " + m.Value[1].ToString().ToLower());
		}

	}
}
