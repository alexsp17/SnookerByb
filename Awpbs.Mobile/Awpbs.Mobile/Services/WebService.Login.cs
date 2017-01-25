using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Awpbs.Mobile
{
    public partial class WebService
    {
        public async Task<bool> Register(string username, string password, string facebookId, string name, string facebookAccessToken)
        {
			string url = WebApiUrl + "Account/Register";
            try
            {
                RegisterWebModel model = new RegisterWebModel
                {
                    ConfirmPassword = password,
                    Password = password,
                    Email = username,
                    FacebookId = facebookId,
                    Name = name,
                    FacebookAccessToken = facebookAccessToken
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model, false);
                return true;
            }
            catch (WebException exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<int?> RegisterFVO(string username, string pin, string name, int venueID)
        {
			string url = WebApiUrl + "Account/RegisterFVO";
            try
            {
                RegisterWebModelFVO model = new RegisterWebModelFVO
                {
                    Pin = pin,
                    Email = username,
                    Name = name,
                    VenueID = venueID,
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model, false);
                int athleteID = JsonConvert.DeserializeObject<int>(json);
                return athleteID;
            }
            catch (WebException exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
            catch (Exception exc)
            {
                LastException = exc;
                return null;
            }
        }

        class TokenResponseModel
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("userName")]
            public string Username { get; set; }

            [JsonProperty(".issued")]
            public string IssuedAt { get; set; }

            [JsonProperty(".expires")]
            public string ExpiresAt { get; set; }
        }

        public async Task<bool?> HasPin(int athleteID)
        {
			string url = WebApiUrl + "People/HasPin?athleteID=" + athleteID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                bool hasPin = JsonConvert.DeserializeObject<bool>(json);
                return hasPin;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool?> VerifyPin(int athleteID, string pin)
        {
			string url = WebApiUrl + "People/VerifyPin";
            try
            {
                VerifyPinWebModel model = new VerifyPinWebModel()
                {
                    AthleteID = athleteID,
                    Pin = pin,
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model,  true);
                bool modelResponse = JsonConvert.DeserializeObject<bool>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<ChangePinResultWebModel> ChangePin(string pin)
        {
            string url = WebApiUrl + "Account/ChangePin";
            try
            {
                ChangePinWebModel model = new ChangePinWebModel()
                {
                    Pin = pin
                };
                string json = await this.sendPostRequestAndReceiveResponse(url, model, true);
                ChangePinResultWebModel result = JsonConvert.DeserializeObject<ChangePinResultWebModel>(json);
                return result;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                lastException = exc;
                return null;
            }
        }

        public async Task<bool> Login(string username, string password, string facebookAccessToken)
        {
            try
            {
                string url = WebApiUrlToken;

                HttpWebRequest request = new HttpWebRequest(new Uri(url));
                request.Timeout = Timeout;
                request.Method = "POST";
                string postString = String.Format("username={0}&password={1}&grant_type=password", 
                    System.Net.WebUtility.UrlEncode(username),
                    System.Net.WebUtility.UrlEncode(password));
                if (string.IsNullOrEmpty(facebookAccessToken) == false)
                    postString += "&facebookToken=" + System.Net.WebUtility.UrlEncode(facebookAccessToken);
                byte[] bytes = Encoding.UTF8.GetBytes(postString);
                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                TokenResponseModel tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(json);
                keyChain.AccessToken = tokenResponse.AccessToken;
                return true;
            }
            catch (Exception exc)
            {
                LastException = exc;
                keyChain.AccessToken = null;
                return false;
            }
        }
    }
}
