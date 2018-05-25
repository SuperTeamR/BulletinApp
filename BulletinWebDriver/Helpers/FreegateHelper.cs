using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class FreegateHelper
    {
        static string APIKey => "c2cd8014f4d11421f85085a21aaf754e";
        static string APIUrl => "c2cd8014f4d11421f85085a21aaf754e";
        public static string ImageToText(string img)
        {
            var result = "";
            DebugHelper.VerboseMode = true;

            var api = new ImageToText
            {
                ClientKey = APIKey,
                FilePath = img,
            };

            if (!api.CreateTask())
                DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
            else if (!api.WaitForResult())
                DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            else
            {
                DebugHelper.Out("Result: " + api.GetTaskSolution().Text, DebugHelper.Type.Success);
                result = api.GetTaskSolution().Text;
            }
            return result;
        }
    }

    #region API models
    public class CreateTaskResponse
    {
        public CreateTaskResponse(dynamic json)
        {
            ErrorId = JsonHelper.ExtractInt(json, "errorId");

            if (ErrorId != null)
            {
                if (ErrorId.Equals(0))
                {
                    TaskId = JsonHelper.ExtractInt(json, "taskId");
                }
                else
                {
                    ErrorCode = JsonHelper.ExtractStr(json, "errorCode");
                    ErrorDescription = JsonHelper.ExtractStr(json, "errorDescription") ?? "(no error description)";
                }
            }
            else
            {
                DebugHelper.Out("Unknown error", DebugHelper.Type.Error);
            }
        }

        public int? ErrorId { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorDescription { get; private set; }
        public int? TaskId { get; private set; }
    }
    public class BalanceResponse
    {
        public BalanceResponse(dynamic json)
        {
            ErrorId = JsonHelper.ExtractInt(json, "errorId");

            if (ErrorId != null)
            {
                if (ErrorId.Equals(0))
                {
                    Balance = JsonHelper.ExtractDouble(json, "balance");
                }
                else
                {
                    ErrorCode = JsonHelper.ExtractStr(json, "errorCode");
                    ErrorDescription = JsonHelper.ExtractStr(json, "errorDescription") ?? "(no error description)";
                }
            }
            else
            {
                DebugHelper.Out("Unknown error", DebugHelper.Type.Error);
            }
        }

        public int? ErrorId { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorDescription { get; private set; }
        public double? Balance { get; private set; }
    }
    public class TaskResultResponse
    {
        public enum StatusType
        {
            Processing,
            Ready
        }

        public TaskResultResponse(dynamic json)
        {
            ErrorId = JsonHelper.ExtractInt(json, "errorId");

            if (ErrorId != null)
                if (ErrorId.Equals(0))
                {
                    Status = ParseStatus(JsonHelper.ExtractStr(json, "status"));

                    if (Status.Equals(StatusType.Ready))
                    {
                        Cost = JsonHelper.ExtractDouble(json, "cost");
                        Ip = JsonHelper.ExtractStr(json, "ip", null, true);
                        SolveCount = JsonHelper.ExtractInt(json, "solveCount", null, true);
                        CreateTime = UnixTimeStampToDateTime(JsonHelper.ExtractDouble(json, "createTime"));
                        EndTime = UnixTimeStampToDateTime(JsonHelper.ExtractDouble(json, "endTime"));

                        Solution = new SolutionData
                        {
                            Token = JsonHelper.ExtractStr(json, "solution", "token", true),
                            GRecaptchaResponse =
                                JsonHelper.ExtractStr(json, "solution", "gRecaptchaResponse", silent: true),
                            GRecaptchaResponseMd5 =
                                JsonHelper.ExtractStr(json, "solution", "gRecaptchaResponseMd5", silent: true),
                            Text = JsonHelper.ExtractStr(json, "solution", "text", silent: true),
                            Url = JsonHelper.ExtractStr(json, "solution", "url", silent: true)
                        };

                        try
                        {
                            Solution.Answers = json.solution.answers;
                        }
                        catch
                        {
                            Solution.Answers = null;
                        }

                        if (Solution.GRecaptchaResponse == null && Solution.Text == null && Solution.Answers == null
                            && Solution.Token == null)
                            DebugHelper.Out("Got no 'solution' field from API", DebugHelper.Type.Error);
                    }
                }
                else
                {
                    ErrorCode = JsonHelper.ExtractStr(json, "errorCode");
                    ErrorDescription = JsonHelper.ExtractStr(json, "errorDescription") ?? "(no error description)";

                    DebugHelper.Out(ErrorDescription, DebugHelper.Type.Error);
                }
            else
                DebugHelper.Out("Unknown error", DebugHelper.Type.Error);
        }

        public int? ErrorId { get; }
        public string ErrorCode { get; private set; }
        public string ErrorDescription { get; }
        public StatusType? Status { get; }
        public SolutionData Solution { get; }
        public double? Cost { get; private set; }
        public string Ip { get; private set; }

        /// <summary>
        ///     Task create time in UTC
        /// </summary>
        public DateTime? CreateTime { get; private set; }

        /// <summary>
        ///     Task end time in UTC
        /// </summary>
        public DateTime? EndTime { get; private set; }

        public int? SolveCount { get; private set; }

        private StatusType? ParseStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return null;

            try
            {
                return (StatusType)Enum.Parse(
                    typeof(StatusType),
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status),
                    true
                );
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? UnixTimeStampToDateTime(double? unixTimeStamp)
        {
            if (unixTimeStamp == null)
                return null;

            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            return dtDateTime.AddSeconds((double)unixTimeStamp).ToUniversalTime();
        }

        public class SolutionData
        {
            public JObject Answers { get; internal set; } // Will be available for CustomCaptcha tasks only!
            public string GRecaptchaResponse { get; internal set; } // Will be available for Recaptcha tasks only!
            public string GRecaptchaResponseMd5 { get; internal set; } // for Recaptcha with isExtended=true property
            public string Text { get; internal set; } // Will be available for ImageToText tasks only!
            public string Url { get; internal set; }
            public string Token { get; internal set; } // Will be available for FunCaptcha tasks only!
        }
    }
    public class ImageToText : AnticaptchaBase, IAnticaptchaTaskProtocol
    {
        public ImageToText()
        {
            BodyBase64 = "";
            Phrase = false;
            Case = false;
            Numeric = false;
            Math = 0;
            MinLength = 0;
            MaxLength = 0;
        }

        public string BodyBase64 { private get; set; }

        public string FilePath
        {
            set
            {
                if (!File.Exists(value))
                {
                    DebugHelper.Out("File " + value + " not found", DebugHelper.Type.Error);
                }
                else
                {
                    BodyBase64 = ImageHelper.ImageFileToBase64String(value);

                    if (BodyBase64 == null)
                    {
                        DebugHelper.Out(
                            "Could not convert the file " + value + " to base64. Is this an image file?",
                            DebugHelper.Type.Error
                            );
                    }
                }
            }
        }

        public bool Phrase { private get; set; }
        public bool Case { private get; set; }
        public bool Numeric { private get; set; }
        public int Math { private get; set; }
        public int MinLength { private get; set; }
        public int MaxLength { private get; set; }
        public int Timeout { private get; set; }

        public override JObject GetPostData()
        {
            if (string.IsNullOrEmpty(BodyBase64))
            {
                return null;
            }

            return new JObject
            {
                {"type", "ImageToTextTask"},
                {"body", BodyBase64.Replace("\r", "").Replace("\n", "")},
                {"phrase", Phrase},
                {"case", Case},
                {"numeric", Numeric},
                {"math", Math},
                {"minLength", MinLength},
                {"maxLength", MaxLength}
            };
        }

        public TaskResultResponse.SolutionData GetTaskSolution()
        {
            return TaskInfo.Solution;
        }
    }
    public interface IAnticaptchaTaskProtocol
    {
        JObject GetPostData();
        TaskResultResponse.SolutionData GetTaskSolution();
    }
    public abstract class AnticaptchaBase
    {
        public enum ProxyTypeOption
        {
            Http,
            Socks4,
            Socks5
        }

        private const string Host = "api.anti-captcha.com";
        private const SchemeType Scheme = SchemeType.Https;
        public string ErrorMessage { get; private set; }
        public int TaskId { get; private set; }
        public string ClientKey { set; private get; }
        public TaskResultResponse TaskInfo { get; protected set; }
        public abstract JObject GetPostData();

        public bool CreateTask()
        {
            var taskJson = GetPostData();

            if (taskJson == null)
            {
                DebugHelper.Out("A task preparing error.", DebugHelper.Type.Error);

                return false;
            }

            var jsonPostData = new JObject();
            jsonPostData["clientKey"] = ClientKey;
            jsonPostData["task"] = taskJson;

            DebugHelper.Out("Connecting to " + Host, DebugHelper.Type.Info);
            dynamic postResult = JsonPostRequest(ApiMethod.CreateTask, jsonPostData);

            if (postResult == null || postResult.Equals(false))
            {
                DebugHelper.Out("API error", DebugHelper.Type.Error);

                return false;
            }

            var response = new CreateTaskResponse(postResult);

            if (!response.ErrorId.Equals(0))
            {
                ErrorMessage = response.ErrorDescription;

                DebugHelper.Out(
                    "API error " + response.ErrorId + ": " + response.ErrorDescription,
                    DebugHelper.Type.Error
                );

                return false;
            }

            if (response.TaskId == null)
            {
                DebugHelper.JsonFieldParseError("taskId", postResult);

                return false;
            }

            TaskId = (int)response.TaskId;
            DebugHelper.Out("Task ID: " + TaskId, DebugHelper.Type.Success);

            return true;
        }

        public bool WaitForResult(int maxSeconds = 120, int currentSecond = 0)
        {
            if (currentSecond >= maxSeconds)
            {
                DebugHelper.Out("Time's out.", DebugHelper.Type.Error);

                return false;
            }

            if (currentSecond.Equals(0))
            {
                DebugHelper.Out("Waiting for 3 seconds...", DebugHelper.Type.Info);
                Thread.Sleep(3000);
            }
            else
            {
                Thread.Sleep(1000);
            }

            DebugHelper.Out("Requesting the task status", DebugHelper.Type.Info);

            var jsonPostData = new JObject();
            jsonPostData["clientKey"] = ClientKey;
            jsonPostData["taskId"] = TaskId;

            dynamic postResult = JsonPostRequest(ApiMethod.GetTaskResult, jsonPostData);

            if (postResult == null || postResult.Equals(false))
            {
                DebugHelper.Out("API error", DebugHelper.Type.Error);

                return false;
            }

            TaskInfo = new TaskResultResponse(postResult);

            if (!TaskInfo.ErrorId.Equals(0))
            {
                ErrorMessage = TaskInfo.ErrorDescription;

                DebugHelper.Out("API error " + TaskInfo.ErrorId + ": " + ErrorMessage, DebugHelper.Type.Error);

                return false;
            }

            if (TaskInfo.Status.Equals(TaskResultResponse.StatusType.Processing))
            {
                DebugHelper.Out("The task is still processing...", DebugHelper.Type.Info);

                return WaitForResult(maxSeconds, currentSecond + 1);
            }

            if (TaskInfo.Status.Equals(TaskResultResponse.StatusType.Ready))
            {
                if (TaskInfo.Solution.GRecaptchaResponse == null && TaskInfo.Solution.Text == null
                    && TaskInfo.Solution.Answers == null && TaskInfo.Solution.Token == null)
                {
                    DebugHelper.Out("Got no 'solution' field from API", DebugHelper.Type.Error);

                    return false;
                }

                DebugHelper.Out("The task is complete!", DebugHelper.Type.Success);

                return true;
            }

            ErrorMessage = "An unknown API status, please update your software";
            DebugHelper.Out(ErrorMessage, DebugHelper.Type.Error);

            return false;
        }

        private dynamic JsonPostRequest(ApiMethod methodName, JObject jsonPostData)
        {
            string error;
            var methodNameStr = char.ToLowerInvariant(methodName.ToString()[0]) + methodName.ToString().Substring(1);

            dynamic data = HttpHelper.Post(
                new Uri(Scheme + "://" + Host + "/" + methodNameStr),
                JsonConvert.SerializeObject(jsonPostData, Formatting.Indented),
                out error
            );

            if (string.IsNullOrEmpty(error))
                if (data == null)
                    error = "Got empty or invalid response from API";
                else
                    return data;
            else
                error = "HTTP or JSON error: " + error;

            DebugHelper.Out(error, DebugHelper.Type.Error);

            return false;
        }

        public double? GetBalance()
        {
            var jsonPostData = new JObject();
            jsonPostData["clientKey"] = ClientKey;

            dynamic postResult = JsonPostRequest(ApiMethod.GetBalance, jsonPostData);

            if (postResult == null || postResult.Equals(false))
            {
                DebugHelper.Out("API error", DebugHelper.Type.Error);

                return null;
            }

            var balanceResponse = new BalanceResponse(postResult);

            if (!balanceResponse.ErrorId.Equals(0))
            {
                ErrorMessage = balanceResponse.ErrorDescription;

                DebugHelper.Out(
                    "API error " + balanceResponse.ErrorId + ": " + balanceResponse.ErrorDescription,
                    DebugHelper.Type.Error
                );

                return null;
            }

            return balanceResponse.Balance;
        }

        private enum ApiMethod
        {
            CreateTask,
            GetTaskResult,
            GetBalance
        }

        private enum SchemeType
        {
            Http,
            Https
        }
    }
    #endregion
}
