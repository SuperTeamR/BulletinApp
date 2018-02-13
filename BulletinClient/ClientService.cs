using BulletinBridge.Commands;
using BulletinBridge.Messages.Base;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient
{
    static class ClientService
    {
        static string serverAddress = "127.0.0.1";
        static TimeSpan PostTimeout = new TimeSpan(0, 0, 100);

        public static TResponse ExecuteQuery<TRequest, TResponse>(TRequest request, CommandApi api)
        {
            var result = default(TResponse);
            DCT.Execute(d =>
            {
                var sm = MessageBase.Create(new SerializationData(request), api);
                var response = Execute<MessageBase>(sm, "Get");
                result = response.Data.Deserialize<TResponse>();
            });
            return result;
          
        }
        static TResponse Execute<TResponse>(object request, string query)
        {
            var result = default(TResponse);
            DCT.Execute(d =>
            {
                var messageString = "";
                var s = new DataContractSerializer(request.GetType());
                using (var ms = new MemoryStream())
                {
                    var xmlWriterSettings = new System.Xml.XmlWriterSettings()
                    {
                        CloseOutput = false,
                        Encoding = Encoding.UTF8,
                        OmitXmlDeclaration = false,
                        Indent = true
                    };
                    using (var xw = System.Xml.XmlWriter.Create(ms, xmlWriterSettings))
                        s.WriteObject(xw, request);
                    messageString = Encoding.UTF8.GetString(ms.ToArray());
                }

                result = SendRequest<TResponse>(serverAddress + @"/" + query, messageString);
                if (result == null) result = default(TResponse);
            });
            return result;
        }

        static TResponse SendRequest<TResponse>(string url, string data)
        {
            using (var hc = new HttpClient())
            {
                var result = default(TResponse);
                HttpContent content = new StringContent(data);
                hc.MaxResponseContentBufferSize = 99999999;
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");
                hc.Timeout = PostTimeout;
                using (var response = hc.PostAsync(url, content).Result)
                {
                    using (var responseContent = response.Content)
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(TResponse));
                        using (var ms = responseContent.ReadAsStreamAsync().Result)
                            result = (TResponse)ser.ReadObject(ms);
                    }
                }
                return result;
            }
               
        }

    }
}
