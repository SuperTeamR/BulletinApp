using BulletinWebDriver.Core;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class OnlineSimHelper
    {
        public static string CreateNumberAvito()
        {
            var result = "";
            try
            {
                var getNum = GetNum(ServiceType.avito);
                var getState= GetState(getNum.tzid);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        #region API methods
        internal static getNumResponse GetNum(ServiceType service)
        {
            getNumResponse result = null;
            DCT.Execute(c => result = GetRequest<getNumResponse>($"https://onlinesim.ru/api/getNum.php?apikey=f02eb880d7930c4a6eec0b39bb893e36&service={service.ToString()}"));
            return result;
        }
        internal static getStatResponse GetState(int tzid)
        {
            getStatResponse[] result = null;
            DCT.Execute(c => result = GetRequest<getStatResponse[]>($"https://onlinesim.ru/api/getState.php?apikey=f02eb880d7930c4a6eec0b39bb893e36&tzid={tzid}"));
            return result != null ? result.FirstOrDefault() : null;
        }
        #endregion
        #region Tools 
        private static T GetRequest<T>(string url)
        {
            T result = default(T);
            DCT.Execute(c => 
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new NullReferenceException("GetRequest exception - URL is empty");
                var response = SendGet(url);
                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("GetRequest exception - response not found");
                result = SerializeJSON<T>(response);
            });
            return result;
        }
        private static T SerializeJSON<T>(string data)
        {
            T result = default(T);
            DCT.Execute(c =>
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    result = (T)serializer.ReadObject(ms);
                }
            });
            return result;
        }
        private static string SendGet(string url)
        {
            var result = "";
            DCT.Execute(c =>
            {
                using (var client = new HttpClient(new HttpClientHandler()))
                {
                    client.Timeout = TimeSpan.FromMilliseconds(10000);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = client.SendAsync(request).Result;
                    result = response.Content.ReadAsStringAsync().Result;
                }
            });
            return result;
        }
        #endregion
    }
    #region API models
    public enum ServiceType
    {
        avito,
        youla,
        mailru,
        gmail,
        yandex,
    }
    #region getNum
    [DataContract]
    public class getNumResponse
    {
        [DataMember]
        public string response { get; set; }
        public getNumState responseEnum { get => EnumHelper.GetValue<getNumState>(response); }
        [DataMember]
        public int tzid { get; set; }
    }
    public enum getNumState
    {
        None = 0,
        Successfull = 1,
        EXCEEDED_CONCURRENT_OPERATIONS = 2,
        NO_NUMBER = 3,
        TIME_INTERVAL_ERROR = 4,
        INTERVAL_CONCURRENT_REQUESTS_ERROR = 5,
        ERROR_NO_SERVICE = 6,
        TRY_AGAIN_LATER = 7,
        NO_FORWARD_FOR_DEFFER = 8,
        NO_NUMBER_FOR_FORWARD = 9,
        ERROR_LENGTH_NUMBER_FOR_FORWARD = 10,
        DUPLICATE_OPERATION = 11,
    }
    #endregion
    #region getState
    [DataContract]
    public class getStatResponse
    {
        [DataMember]
        public string response { get; set; }
        public getStateState responseEnum { get => EnumHelper.GetValue<getStateState>(response); }
        [DataMember]
        public int tzid { get; set; }
        [DataMember]
        public string service { get; set; }
        [DataMember]
        public string number { get; set; }
        [DataMember]
        public string msg { get; set; }
        [DataMember]
        public string time { get; set; }
        [DataMember]
        public string form { get; set; }
        [DataMember]
        public string forward_status { get; set; }
        [DataMember]
        public string forward_number { get; set; }
        [DataMember]
        public string country { get; set; }
    }
    public enum getStateState
    {
        None = 0,
        Successfull = 1,
        WARNING_NO_NUMS = 2,
        TZ_INPOOL = 3,
        TZ_NUM_WAIT = 4,
        TZ_NUM_ANSWER = 5,
        TZ_OVER_EMPTY = 6,
        TZ_OVER_OK = 7,
        ERROR_NO_TZID = 8,
        ERROR_NO_OPERATIONS = 9,
        ACCOUNT_IDENTIFICATION_REQUIRED = 10,
    }
    #endregion
    #endregion
}
