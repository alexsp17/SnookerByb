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
        public readonly string WebApiUrl = 
            Config.IsProduction ? "https://awpbswebapi2.azurewebsites.net/api/" : "https://awpbswebapiDev.azurewebsites.net/api/";
        public readonly string WebApiUrlToken = 
            Config.IsProduction ? "https://awpbswebapi2.azurewebsites.net/Token" : "https://awpbswebapiDev.azurewebsites.net/Token";

        public const int Timeout = 5000;

        private IKeyChain keyChain;

        private string accessToken {  get { return keyChain.AccessToken; } }

        public event EventHandler AccessTokenExpired;

        public Exception LastException
        {
            get
            {
                return this.lastException;
            }
            private set
            {
                this.lastException = value;

				Console.WriteLine ("Webservice class - EXCEPTION: " + TraceHelper.ExceptionToString(value));

                // if the exception indicates that the access token is expired => fire AccessTokenExpired event
                bool is401 = false;
                WebException webException = this.lastException as WebException;
                if (webException != null)
                {
                    if (webException.Message != null && webException.Message.Contains("401") && webException.Message.Contains("Unauthorized"))
                        is401 = true;
                    System.Net.HttpWebResponse response = webException.Response as System.Net.HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                        is401 = true;
                }
                if (is401)
                {
                    if (AccessTokenExpired != null)
                        AccessTokenExpired(this, EventArgs.Empty);
                }
            }
        }
        private Exception lastException;

		public bool IsLastExceptionDueToInternetIssues
		{
			get
			{
				if (this.lastException == null)
					return false;
				var webException = lastException as WebException;
				if (webException == null)
					return false;
				if (webException.Status == WebExceptionStatus.NameResolutionFailure) // noticed this on Android emulator
					return true;
				if (webException.Status == WebExceptionStatus.ReceiveFailure) // noticed this on Android emulator
					return true;
				if (webException.Status == WebExceptionStatus.ConnectFailure) // not sure about this...
					return true;
				return false;
			}
		}

		public string LastExceptionUrl
		{
			get;
			private set;
		}

        public WebService(IKeyChain keyChain)
        {
            this.keyChain = keyChain;
        }

		private async Task<string> sendPostRequestAndReceiveResponse(string url, object model, bool shouldAuthorize)
		{
			Console.WriteLine ("Webservice class - POST - " + url);
			
			HttpWebRequest request = new HttpWebRequest(new Uri(url));
			request.Timeout = Timeout;
			request.ReadWriteTimeout = Timeout;
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Accept = "application/json";
			if (shouldAuthorize)
				request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

			string requestJson = JsonConvert.SerializeObject(model);
			byte[] bytes = Encoding.UTF8.GetBytes(requestJson);
			using (Stream stream = await request.GetRequestStreamAsync())
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
			string json;
			using (Stream responseStream = httpResponse.GetResponseStream())
			{
				json = new StreamReader(responseStream).ReadToEnd();
			}

			return json;
		}

		private async Task<string> sendPostRequestAndReceiveResponse(string url, Stream stream,bool shouldAuthorize)
		{
			Console.WriteLine ("Webservice class - POST - " + url);
			
			HttpWebRequest request = new HttpWebRequest(new Uri(url));
			request.Timeout = 30000; // 30 sec timeout
			request.ReadWriteTimeout = Timeout;
			request.Method = "POST";
			request.Accept = "application/json";
			if (shouldAuthorize)
				request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

			byte[] bytes = new BinaryReader(stream).ReadBytes((int)stream.Length);
			using (Stream stream1 = await request.GetRequestStreamAsync())
			{
				stream1.Write(bytes, 0, bytes.Length);
			}

			HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
			string json;
			using (Stream responseStream = httpResponse.GetResponseStream())
			{
				json = new StreamReader(responseStream).ReadToEnd();
			}

			return json;
		}

		private async Task<string> sendPostRequestAndReceiveResponse(string url, bool shouldAuthorize)
		{
			Console.WriteLine ("Webservice class - POST - " + url);
			
			HttpWebRequest request = new HttpWebRequest(new Uri(url));
			request.Timeout = Timeout;
			request.ReadWriteTimeout = Timeout;
			request.Method = "POST";
			request.Accept = "application/json";
			if (shouldAuthorize)
				request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));
			request.ContentLength = 0;

			HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
			string json;
			using (Stream responseStream = httpResponse.GetResponseStream())
			{
				json = new StreamReader(responseStream).ReadToEnd();
			}

			return json;
		}

		private async Task<string> sendGetRequestAndReceiveResponse(string url, bool shouldAuthorize)
		{
			Console.WriteLine ("Webservice class - GET - " + url);
			
			HttpWebRequest request = new HttpWebRequest(new Uri(url));
			request.Timeout = Timeout;
			request.ReadWriteTimeout = Timeout;
			request.Method = "GET";
			request.Accept = "application/json";
			if (shouldAuthorize)
				request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

			HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
			string json;
			using (Stream responseStream = httpResponse.GetResponseStream())
			{
				json = new StreamReader(responseStream).ReadToEnd();
			}

			return json;
		}
    }
}
