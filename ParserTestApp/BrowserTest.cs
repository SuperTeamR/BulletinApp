using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserTestApp
{
    public class BrowserTest
    {
        public void Run()
        {
            GetPage("www.google.ru", ShowPage);
        }

        void GetPage(string url, Action<string> afterCompleted = null)
        {
            WebWorker.DownloadPage(url, afterCompleted);
        }


        void ShowPage(string text)
        {
            var t = text;
            t = t;
        }
    }
}
