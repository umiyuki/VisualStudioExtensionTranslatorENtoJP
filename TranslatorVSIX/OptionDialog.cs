using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TranslatorVSIX
{
    class OptionDialog : DialogPage
    {
        [Category("オプション")]
        [DisplayName("API Key")]
        [Description("Google Cloud Translation APIのAPIKeyを入力してください")]
        public string apikey { get; set; }

        [Category("裏オプション")]
        [DisplayName("Google App Script API")]
        [Description("Google App Scriptで作成したWebアプリケーションのURLを入力してください")]
        public string GASEndpoint
        {
            get;set;
        }

        private string sourceLang = "en";
        [Category("オプション")]
        [DisplayName("入力言語")]
        [Description("入力する言語の言語コードを入力してください")]
        public string SourceLang
        {
            get { return sourceLang; }
            set { sourceLang = value; }
        }

        private string destinationLang = "ja";
        [Category("オプション")]
        [DisplayName("出力言語")]
        [Description("出力する言語の言語コードを入力してください")]
        public string DestinationLang
        {
            get { return destinationLang; }
            set { destinationLang = value; }
        }
    }
}
