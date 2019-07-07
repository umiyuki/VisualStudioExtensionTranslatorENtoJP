//------------------------------------------------------------------------------
// <copyright file="Command1.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using VSTranslator.Translation;
using VSTranslator.Translation.Bing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace TranslatorVSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class Translate
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("add19930-1bc1-4eba-a1fb-43d9858010aa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Translate"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private Translate(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static Translate Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new Translate(package);
        }

        static Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

        public static string[] SubstringAtCount(string self, int count)
        {

            var result = new List<string>();
            var length = (int)Math.Ceiling((double)sjisEnc.GetByteCount(self) / count);

            var chars = self.ToCharArray();

            string str = "";
            int strcount = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                str += chars[i];
                int bytes = System.Text.Encoding.UTF8.GetByteCount(chars[i].ToString());
                strcount += bytes;
                if (strcount >= count)//count超えたら改行
                {
                    result.Add(str);
                    str = "";
                    strcount = 0;
                }
            }

            if (str != "")//最後の行
            {
                result.Add(str);
            }

            /*
            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (self.Length <= start)
                {
                    break;
                }
                if (self.Length < start + count)
                {
                    result.Add(self.Substring(start));
                }
                else
                {
                    result.Add(self.Substring(start, count));
                }
            }*/

            return result.ToArray();
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        // この関数は、メニュー項目がクリックされたときにコマンドを実行するために使用される
        // コールバックです。 OleMenuCommandServiceサービスとMenuCommandクラスを使用して、メニュー項目
        // がこの関数にどのように関連付けられているかを確認するには、コンストラクターを参照
        // してください。
        // この関数は、メニュー項目がクリックされたときにコマンドを実行するために使用される
        // コールバックです。 OleMenuCommandServiceサービスとMenuCommandクラスを使用して、メニュー項目
        // がこの関数にどのように関連付けられているかを確認するには、コンストラクターを参照
        // してください。
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "Translate";

            // Show a message box to prove we were here
            // 私たちがここにいたことを証明するメッセージボックスを表示する
            /*
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                */

            DTE dte = (DTE)this.ServiceProvider.GetService(typeof(DTE));
            if (dte.ActiveDocument != null)
            {
                var selection = (TextSelection)dte.ActiveDocument.Selection;
                string text = selection.Text;

                if (text != "")
                {
                    //テキストを整形する
                    text = text.Replace("\n", "");//改行削除
                    text = System.Text.RegularExpressions.Regex.Replace(text, "//+", "");//コメント削除
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");//連続するスペースを一つに減らす
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"\n\z", "");//末尾の改行削除

                    //ダイアログから設定読み込み
                    IVsShell shell = this.ServiceProvider.GetService(typeof(SVsShell)) as IVsShell;
                    if (shell == null) return;
                    IVsPackage package = null;
                    Guid PackageToBeLoadedGuid =
                        new Guid(VSPackage1.PackageGuidString);
                    shell.LoadPackage(ref PackageToBeLoadedGuid, out package);
                    VSPackage1 vsPackage = (VSPackage1)package;
                    string apikey = vsPackage.ApiKey;
                    string gasEndpoint = vsPackage.GASEndpoint;
                    string sourceLange = vsPackage.SourceLang;
                    string destinationLang = vsPackage.DestinationLang;

                    TranslationResult result = null;

                    if (!string.IsNullOrEmpty(apikey))
                    {
                        GoogleTranslator translator = new GoogleTranslator();
                        result = translator.GetTranslation(text, sourceLange, destinationLang, apikey);
                    }
                    else if (!string.IsNullOrEmpty(gasEndpoint))
                    {
                        GoogleTranslator translator = new GoogleTranslator();
                        result = translator.GetTranslationFromGoogleAppScript(text, sourceLange, destinationLang, gasEndpoint);
                    }
                    else
                    {
                        VsShellUtilities.ShowMessageBox(
                        this.ServiceProvider,
                        "設定画面(ツール→オプション→TranslatorENtoJP→General)からGoogle Cloud Translation APIのAPIキーを入力してください",
                        "apiキーが未設定です",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return;
                    }

                    //120バイトごとに改行
                    var strs = SubstringAtCount(result.Sentences[0], 120);

                    string finalstr = "";
                    for (int i = 0; i < strs.Length; i++)
                    {
                        finalstr += "\n// " + strs[i];
                    }

                    selection.Insert(selection.Text + finalstr);
                    selection.SmartFormat();
                    //selection.Text += finalstr;

                    /*
                    BingConnector conn = new BingConnector();
                    string response = conn.GetTranslations(text, "en", "ja");
                    JObject jObj = JObject.Parse(response);
                    //Assert.IsTrue(jObj["From"] != null && jObj["Translations"] != null);

                    //text = ">>" + text + "<<";
                    System.Console.Write(jObj["Translations"]);
                    if (jObj["From"] != null && jObj["Translations"] != null)
                    {
                        selection.Text += "\n//" + jObj["Translations"][0]["TranslatedText"];
                    }
                    else
                    {
                        VsShellUtilities.ShowMessageBox(
                            this.ServiceProvider,
                            "some error.",
                            "error",
                            OLEMSGICON.OLEMSGICON_INFO,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    }*/
                }
            }

        }
    }
}
