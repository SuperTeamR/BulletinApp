using BulletinWebDriver.Core;
using CollectorModels;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    class ProxyHelper
    {
        public static ProxyCardCheckCache GetProxy(string URL, IEnumerable<string> exceptionString, int timeout = 5000)
        {
            ProxyCardCheckCache result = null;
            DCT.Execute(c =>
            {
                ConsoleHelper.SendMessage($"GetProxy started");
                while (true)
                {
                    var proxy = CollectorModels.Service.ProxyClientHelper.Next();
                    if (proxy == null)
                    {
                        ConsoleHelper.SendMessage($"Proxy service crash or not available now");
                        return;
                    }
                    var text = proxyCheck(URL, proxy, timeout);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (exceptionString != null && exceptionString.Any())
                        {
                            foreach (var str in exceptionString)
                            {
                                if (text.Contains(str))
                                    continue;
                            }
                        }
                        result = proxy;
                        ConsoleHelper.SendMessage($"Get proxy completed");
                        return;
                    }
                }
            });
            return result;
        }
        private static string proxyCheck(string URL, ProxyCardCheckCache proxyCache, int timeout = 5000)
        {
            var result = "";
            HttpStatusCode status = HttpStatusCode.RequestTimeout;
            try
            {
                var text = "";
                var proxy = new WebProxy(proxyCache.Address, Convert.ToInt32(proxyCache.Port));
                var sw = new Stopwatch();
                sw.Start();
                using (var client = new HttpClient(new HttpClientHandler() { Proxy = proxy }))
                {
                    client.Timeout = TimeSpan.FromMilliseconds(timeout);
                    var request = new HttpRequestMessage(HttpMethod.Get, URL);
                    var response = client.SendAsync(request).Result;
                    status = response.StatusCode;
                    text = response.Content.ReadAsStringAsync().Result;
                }
                sw.Stop();
                if (status != null && status == HttpStatusCode.OK)
                {
                    var p = Convert.ToInt32(sw.ElapsedMilliseconds);
                    if (p < timeout)
                        result = text;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendMessage($"Proxy don't worked - {proxyCache.Address}:{proxyCache.Port}. Connect status {status}");
            }
            return result;
        }
        //public static bool CheckResponse()
        //{
        //    var result = false;
        //    HttpWebResponse response = null;
        //    DCT.Execute(d =>
        //    {
        //        WebRequest.DefaultWebProxy = new WebProxy
        //        {
        //            Address = new Uri("http://" + proxy.Address + ":" + proxy.Port)
        //        };
        //        var request = WebRequest.Create("https://avito.ru");
        //        request.Timeout = 5000;
        //        try
        //        {
        //            response = (HttpWebResponse)request.GetResponse();
        //            if (response.StatusCode == HttpStatusCode.OK)
        //            {
        //                result = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        finally
        //        {
        //            if (response != null) response.Close();
        //        }

        //    });
        //    return result;
        //}
    }
}
