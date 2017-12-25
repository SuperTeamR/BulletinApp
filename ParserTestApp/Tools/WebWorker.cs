using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ParserTestApp.Tools
{
    public static class WebWorker
    {
        #region STA Thread tool
        private static Thread WorkThread { get; set; }
        public static WebBrowser WebBrowser { get; set; }

        public static HtmlDocument WebDocument => WebBrowser.Document;

        static ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        public static Action IntializationComplite { get; set; }

        private static void WorkThreadCheckOrCreate()
        {
            _DCT.Execute(data =>
            {
                if (WorkThread == null || !WorkThread.IsAlive)
                {
                    WorkThread = new Thread(() =>
                    {
                        if (WebBrowser != null)
                        {
                            WebBrowser.Dispose();
                            WebBrowser = null;
                        }
                        WebBrowser = new WebBrowser();
                        WebBrowser.ScriptErrorsSuppressed = true;
                        while (true)
                        {
                            Action next;
                            var result = queue.TryDequeue(out next);
                            if (result && next != null)
                            {
                                next();
                                _DCT.SendInfo("STA Action execute");
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                _DCT.SendInfo("STA iteration complete");
                            }
                        }
                    });
                    WorkThread.SetApartmentState(ApartmentState.STA);
                    WorkThread.IsBackground = true;
                    WorkThread.Start();
                    IntializationComplite?.Invoke();
                }
            }, _DCTGroup.WebWorker);
        }

        public static void Execute(Action executeAction, Action completeAction = null)
        {
            _DCT.Execute(data2 =>
            {
                if (queue == null)
                    queue = new ConcurrentQueue<Action>();
                WorkThreadCheckOrCreate();
                queue.Enqueue(() =>
                _DCT.Execute(data =>
                {
                    if (executeAction == null) throw new NullReferenceException("WebWorker.Execute Параметр не может быть пустым");
                    executeAction?.Invoke();
                    completeAction?.Invoke();
                }, _DCTGroup.WebWorker));
            }, _DCTGroup.WebWorker);
        }
        #endregion



        #region BaseWebWorker

        public static void DownloadPage(string url, Action<string> compliteAction)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new NullReferenceException("DownloadPage url can't be null!");
            Execute(() =>
            {
                WebBrowser.Navigate(url);
                Wait();
                var stream = WebBrowser.DocumentStream;
                var result = "";
                using (var sr = new StreamReader(stream))
                    result = sr.ReadToEnd();
                compliteAction?.Invoke(result);
            });
        }
        private static void Wait()
        {
            try
            {
                var date = DateTime.Now;
                while (true)
                {
                    Application.DoEvents();
                    if (date.AddSeconds(10) < DateTime.Now) break;
                    var state = WebBrowser.ReadyState;
                    if (state == WebBrowserReadyState.Complete)
                        break;
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion
    }
}
