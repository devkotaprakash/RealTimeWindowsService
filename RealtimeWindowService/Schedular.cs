using Newtonsoft.Json;
using RealTimeProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RealtimeWindowService
{
    public partial class Schedular : ServiceBase
    {
        static Random random = new Random();
        static HttpClient client = new HttpClient();
        private System.Timers.Timer timer1 = null;
        public Schedular()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new System.Timers.Timer();
            this.timer1.Interval = 5000;
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.WriteErrorLog("Fluent Home windows service started");




        }
        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            const int duration = 60; // Length of time in minutes to push data
            const int pauseInterval = 2; // Frequency in seconds that data will be pushed
            const string timeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ"; // Time format required by Power BI

            GetToken();
         
         
            GenerateObservations(duration, pauseInterval, timeFormat);

            Library.WriteErrorLog("Timer ticked and some job has been done successfully");
        }
        protected override void OnStop()

        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Service stopped");
        }

        public static void GenerateObservations(int duration, int pauseInterval, string timeFormat)
        {
            List<SkillRealTimeModel> totalList = GetSkillActivities();
            SkillSummaryList skillSummaryList = GetRTDSkillsSummary(DateTime.Now.AddHours(-4).ToString(), DateTime.Now.ToString());

            #region  CustomerLoyalty


            List<int> customerLoyaltyIds = new List<int> { 352669, 352657, 346984, 355141, 355474, 355475, 355506, 352670 };
            //for holdtime and longestqueue
            List<Skillsummary> customerSkillList = skillSummaryList.skillSummaries.Where(t => customerLoyaltyIds.Contains(Convert.ToInt32(t.skillId))).ToList();
            Dictionary<string, int> handLforcustomerloyalty = GetHoldTimeAndLongestQueue(customerSkillList);
            //for agents state and summary
            List<SkillRealTimeModel> CustomerLoyaltyList = totalList.Where(t => customerLoyaltyIds.Contains(t.SkillId)).ToList();
            SkillRealTimeModel customerLoyaltyModel = GetStreamingDataModel(CustomerLoyaltyList, 277692, "Customer Loyalty");
            customerLoyaltyModel.HoldTime = handLforcustomerloyalty["HoldTime"];
            customerLoyaltyModel.LongestQueue = handLforcustomerloyalty["LongestQueue"];

            var jsonString = JsonConvert.SerializeObject(customerLoyaltyModel);
            var postToPowerBi = HttpPostAsync("https://api.powerbi.com/beta/4b700a32-7025-4159-a131-8e9c9057f203/datasets/7a938ee1-f3f8-47b7-89c6-9f185235d469/rows?key=wfUHBqHwYBQH0wLyBIHLekvIeZ7Yt6KE8zQ7MlfTUf4O%2FNMgOVEhmHigfBIyuuRlV7Ueg0Tcbf8N3Lm2vyaq2A%3D%3D", "[" + jsonString + "]"); // Add brackets for Power BI



            #endregion



            #region  DataEntry
            List<int> dateEntryIds = new List<int> { 352675, 352676, 352678, 352679, 491259 };

            List<Skillsummary> dataentrySkillList = skillSummaryList.skillSummaries.Where(t => dateEntryIds.Contains(Convert.ToInt32(t.skillId))).ToList();
            Dictionary<string, int> handLfordataentry = GetHoldTimeAndLongestQueue(dataentrySkillList);
            List<SkillRealTimeModel> DataEntryList = totalList.Where(t => dateEntryIds.Contains(t.SkillId)).ToList();
            SkillRealTimeModel DataEntryModel = GetStreamingDataModel(DataEntryList, 277722, "Fluent - DataEntry");
            DataEntryModel.HoldTime = handLfordataentry["HoldTime"];
            DataEntryModel.LongestQueue = handLfordataentry["LongestQueue"];
            var dataentryjsonString = JsonConvert.SerializeObject(DataEntryModel);
            var postTodataentryPowerBi = HttpPostAsync("https://api.powerbi.com/beta/4b700a32-7025-4159-a131-8e9c9057f203/datasets/677f0ce8-bde2-42f5-a201-0d5f398c1623/rows?key=LtLYkLl8zjktsKDNrC8ominN9ClAMfLCkPzrMmbQggPxnZIqQcn7I2hUtdVDvZNnLnCAgtsIZR0gWc4ZtC9UOQ%3D%3D", "[" + dataentryjsonString + "]"); // Add brackets for Power BI
            #endregion





        }
        static SkillRealTimeModel GetStreamingDataModel(List<SkillRealTimeModel> Record, int campaignid, string campaignName)
        {
            SkillRealTimeModel returnRecord = new SkillRealTimeModel();
            returnRecord.CampaignId = campaignid;
            returnRecord.CampaignName = campaignName;
            returnRecord.AgentsACW = Record.Sum(item => item.AgentsACW);
            returnRecord.AgentsAvailable = Record.Sum(item => item.AgentsAvailable);
            returnRecord.AgentsIdle = Record.Sum(item => item.AgentsIdle);
            returnRecord.AgentsLoggedIn = Record.Sum(item => item.AgentsLoggedIn);
            returnRecord.AgentsUnavailable = Record.Sum(item => item.AgentsUnavailable);
            returnRecord.AgentsWorking = Record.Sum(item => item.AgentsWorking);
            returnRecord.ContactsActive = Record.Sum(item => item.ContactsActive);
            returnRecord.QueueCount = Record.Sum(item => item.QueueCount);
            returnRecord.InSLA = Record.Sum(item => item.InSLA);
            returnRecord.OutSLA = Record.Sum(item => item.OutSLA);
            returnRecord.ServiceLevel = Record.Sum(item => item.ServiceLevel) / Record.Count;
            returnRecord.ServiceLevelGoal = Record.Sum(item => item.ServiceLevelGoal) / Record.Count;
            returnRecord.ServiceLevelThreshold = Record.Sum(item => item.ServiceLevelThreshold) / Record.Count;

            return returnRecord;
        }
        static async Task<HttpResponseMessage> HttpPostAsync(string url, string data)
        {
            // Construct an HttpContent object from StringContent
            HttpContent content = new StringContent(data);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        static DateTime GetDateTimeUtc()
        {
            return DateTime.UtcNow;
        }

        #region InContactAPI functions
        #region InContactCredentials
        private string _vendorName = "Outcode";
        private string _appName = "CustomApi";
        private string _busNo = "4594986";
        private string _username = "pneupane@outcodesoftware.com";
        private string _password = "Insight123!";// this is test 
        private string _scope = "AdminApi AgentApi AuthenticationApi PatronApi RealTimeApi";
        private static string accessToken;
        private static string baseURL;
        private static string scope;
        private string skillId;
        private TokenResponse _tokenResponse;
        #endregion
        [Serializable]
        public struct TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
            public string resource_server_base_uri { get; set; }
            public string refresh_token_server_uri { get; set; }
            public int agent_id { get; set; }
            public int team_id { get; set; }
            public string SkillId { get; set; }
            public string baseURL { get; set; }
            public string accessToken { get; set; }
        }

        private void GetToken()
        {
            //Authorization endpoint
            string endpoint =
              @"https://api.incontact.com/InContactAuthorizationServer/token";
            //Encoded request string
            string AuthToken = _appName + "@" + _vendorName + ":" + _busNo;
            string encodedAuthToken = Convert.ToBase64String(
                  System.Text.Encoding.UTF8.GetBytes(AuthToken));

            // Since this is a desktop application that is not running in a 
            // browser we will use  the password "grant type" to complete the 
            // post data. 
            string postData = "{\"grant_type\":\"password\",\"username\":\"" +
                              _username +
                              "\",\"password\":\"" + _password +
                              "\",\"scope\":\"" + _scope + "\"}";

            //Create the request object
            var request =
                  (System.Net.HttpWebRequest)System.Net.WebRequest.Create(endpoint);
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            //Add headers to the request object
            request.Headers.Add("Authorization", "basic " + encodedAuthToken);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            //Write Post Data to the request stream
            var encoding = new System.Text.UTF8Encoding();
            var bytes =
                  System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(postData);
            request.ContentLength = bytes.Length;
            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            // Make the request.
            // Much of the error handling was omitted from this example for the 
            // sake of brevity.
            int statusCode;
            string statusDescription;
            string responseBody;
            using (var response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                int numericStatusCode = (int)response.StatusCode;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // Failure.
                    statusCode = numericStatusCode;
                    statusDescription = String.Format("Request failed. " +
                        "Received HTTP {0}", response.StatusCode);
                    responseBody = string.Empty;
                }
                else
                {
                    // Success.
                    var responseValue = string.Empty;
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader =
                                new System.IO.StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                        }
                    }
                    statusCode = numericStatusCode;
                    statusDescription = response.StatusCode.ToString();
                    responseBody = responseValue;

                }
            }

            //Deserialize the response
            if (!string.IsNullOrEmpty(responseBody))
            {
                // Place a breakpoint here to debug the tokenResponse struct as it
                // fills up with the deserialized JSON response.  Other JSON 
                // responses can be handled similarly for cleaner output and 
                // access to the data.   
                System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer
                  = new System.Web.Script.Serialization.JavaScriptSerializer();
                _tokenResponse =
                    javaScriptSerializer.Deserialize<TokenResponse>(responseBody);

                // Save or store the token.  
                // For this example we will store it in the Instance variables.
                if (!string.IsNullOrEmpty(_tokenResponse.access_token))
                {
                    accessToken = _tokenResponse.access_token.ToString();
                    baseURL = _tokenResponse.resource_server_base_uri;
                    scope = _tokenResponse.scope;
                }
                else
                {
                    //Error branch for empty response
                }
            }
        }


        protected static TotalAgentStates GetRTDAgentStates()
        {
            TotalAgentStates total = new TotalAgentStates();

            string updatedSince = DateTime.Now.AddDays(-1).ToString();
            //Test to see if you have obtained a token
            if (!string.IsNullOrEmpty(accessToken))
            {
                string apiURL = "/services/v11.0/agents/states?updatedSince=" + updatedSince;
                //baseURL was returned with your successful token request
                string endpoint = baseURL + apiURL;
                string authorizationHeader = "Authorization:bearer " + accessToken;

                //Create the request object
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(endpoint);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                //Add any and all necessary headers
                request.Headers.Add("Authorization", "bearer " + accessToken);
                //Accept is a reserved header, so you must modify it rather than add
                request.Accept = "application/json, text/javascript, */*; q=0.01";

                //Make the request
                try
                {
                    using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                    {



                        string responseBody = "";
                        int statusCode = (int)response.StatusCode;
                        string statusDescription = response.StatusDescription;
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new System.IO.StreamReader(responseStream))
                                {

                                    responseBody = reader.ReadToEnd();

                                }
                            }
                        }

                        AgentStateList tmp = JsonConvert.DeserializeObject<AgentStateList>(responseBody);

                        total.TotalActive = tmp.agentStates.Where(item => item.isActive = true).Count();
                        total.TotalInActive = tmp.agentStates.Where(item => item.isActive = false).Count();
                        total.TotalAvailable = tmp.agentStates.Where(item => item.agentStateName == "Available").Count();
                        total.TotalUnAvailable = tmp.agentStates.Where(item => item.agentStateName == "LoggedOut").Count();
                        total.TotalInBound = tmp.agentStates.Where(item => item.agentStateName == "InboundContact").Count();
                        total.TotalOutBound = tmp.agentStates.Where(item => item.agentStateName == "OutboundContact").Count();



                    }
                }
                catch (System.Net.WebException webException)
                {


                }
                catch (Exception ex)
                {


                }
            }
            else
            {
                //No token - get one or handle error
            }
            return total;
        }


        public class TotalAgentStates
        {
            public int TotalAvailable { get; set; }
            public int TotalUnAvailable { get; set; }
            public int TotalActive { get; set; }
            public int TotalInActive { get; set; }
            public int TotalInBound { get; set; }
            public int TotalOutBound { get; set; }

        }


        protected static List<SkillRealTimeModel> GetSkillActivities()
        {
            List<SkillRealTimeModel> record = new List<SkillRealTimeModel>();
            string updatedSince = DateTime.Now.AddDays(-1).ToShortDateString();
            //Test to see if you have obtained a token
            if (!string.IsNullOrEmpty(accessToken))
            {
                string apiURL = "/services/v11.0/skills/activity?fields=" + "&updatedSince=" + updatedSince;
                //baseURL was returned with your successful token request
                string endpoint = baseURL + apiURL;
                string authorizationHeader = "Authorization:bearer " + accessToken;

                //Create the request object
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(endpoint);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                //Add any and all necessary headers
                request.Headers.Add("Authorization", "bearer " + accessToken);
                //Accept is a reserved header, so you must modify it rather than add
                request.Accept = "application/json, text/javascript, */*; q=0.01";

                //Make the request
                try
                {
                    using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                    {
                        string responseBody = "";
                        int statusCode = (int)response.StatusCode;
                        string statusDescription = response.StatusDescription;
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new System.IO.StreamReader(responseStream))
                                {
                                    responseBody = reader.ReadToEnd();
                                }
                            }
                        }
                        SkillActivityList tmp = JsonConvert.DeserializeObject<SkillActivityList>(responseBody);
                        record = SkillActivtyToSkillActivityRealTimeModel(tmp);

                    }
                }
                catch (System.Net.WebException webException)
                {

                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            return record;
        }





        public static List<SkillRealTimeModel> SkillActivtyToSkillActivityRealTimeModel(SkillActivityList Record)
        {
            List<SkillRealTimeModel> returnRecord = new List<SkillRealTimeModel>();
            foreach (var item in Record.skillActivity)
            {
                SkillRealTimeModel single = new SkillRealTimeModel
                {

                    AgentsACW = item.agentsACW,
                    AgentsAvailable = item.agentsAvailable,
                    AgentsIdle = item.agentsIdle,
                    AgentsLoggedIn = item.agentsLoggedIn,
                    AgentsUnavailable = item.agentsUnavailable,
                    ContactsActive = item.contactsActive,
                    QueueCount = item.queueCount,
                    InSLA = item.inSLA,
                    OutSLA = item.outSLA,
                    ServiceLevel = item.serviceLevel,
                    ServiceLevelGoal = item.serviceLevelGoal,
                    ServiceLevelThreshold = item.serviceLevelThreshold,
                    CampaignId = item.campaignId,
                    CampaignName = item.campaignName,
                    SkillId=item.skillId,
                    SkillName=item.skillName


                };
                returnRecord.Add(single);

            }
            return returnRecord;


        }




        protected static SkillSummaryList GetRTDSkillsSummary(string start, string end)
        {

            SkillSummaryList tmp = new SkillSummaryList();
            if (!string.IsNullOrEmpty(accessToken))
            {
                string apiURL = "/services/v11.0/skills/summary?startDate=" + start +
                                "&endDate=" + end;

                //baseURL was returned with your successful token request
                string endpoint = baseURL + apiURL;
                string authorizationHeader = "Authorization:bearer " + accessToken;

                //Create the request object
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(endpoint);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                //Add any and all necessary headers
                request.Headers.Add("Authorization", "bearer " + accessToken);
                //Accept is a reserved header, so you must modify it rather than add
                request.Accept = "application/json, text/javascript, */*; q=0.01";

                //Make the request
                try
                {
                    using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                    {

                        string responseBody = "";
                        int statusCode = (int)response.StatusCode;
                        string statusDescription = response.StatusDescription;
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new System.IO.StreamReader(responseStream))
                                {
                                    responseBody = reader.ReadToEnd();
                                }
                            }
                            tmp = JsonConvert.DeserializeObject<SkillSummaryList>(responseBody);


                        }
                    }
                }
                catch (System.Net.WebException webException)
                {
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }
            return tmp;
        }


        static Dictionary<string, int> GetHoldTimeAndLongestQueue(List<Skillsummary> record)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int holdTime = (record.Sum(t => t.holdTime)) / 60;
            int longestQueue = (record.Max(t => t.longestQueueDur)) / 60;
            dictionary.Add("HoldTime", holdTime);
            dictionary.Add("LongestQueue", longestQueue);
            return dictionary;

        }
        #endregion
    }
}
