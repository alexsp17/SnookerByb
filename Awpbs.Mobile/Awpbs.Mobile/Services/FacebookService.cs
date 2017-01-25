using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public class BasicFacebookProfileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
    }

    public abstract class FacebookService
    {
        public event EventHandler LoginSuccessful;
        public event EventHandler LoginFailed;

        public abstract bool IsLoggedIn { get; }

        public abstract string FacebookAccessToken { get; }

        public abstract bool StartLogin();

        public abstract void Share(string text, EventHandler<bool> done);

        public void OnLoginSucessful()
        {
            if (this.LoginSuccessful != null)
                this.LoginSuccessful(this, EventArgs.Empty);
        }

        public void OnLoginFailed()
        {
            if (this.LoginFailed != null)
                this.LoginFailed(this, EventArgs.Empty);
        }

        public class FacebookGraphMeResult
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string EMail { get; set; }
        }

        public virtual async Task<BasicFacebookProfileInfo> GetBasicProfileInfo()
        {
            try
            {
                string accessToken = FacebookAccessToken;
                string url = string.Format("https://graph.facebook.com/v2.4/me?access_token={0}&fields=id,name,email", accessToken);

                HttpWebRequest request = new HttpWebRequest(new Uri(url));
                request.Method = "GET";
                request.Accept = "application/json";

                HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                FacebookGraphMeResult result = JsonConvert.DeserializeObject<FacebookGraphMeResult>(json);
                return new BasicFacebookProfileInfo()
                {
                    Id = result.ID,
                    Name = result.Name,
                    EMail = result.EMail
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public virtual string Test()
        //{
        //    try
        //    {
        //        if (FacebookAccessToken == null)
        //        {
        //          //  Facebook.CoreKit.AccessToken.RefreshCurrentAccessToken(null);
        //            if (FacebookAccessToken == null)
        //                throw new Exception("still don't have access token");
        //        }

        //        string accessToken = FacebookAccessToken;
        //        string url = string.Format("https://graph.facebook.com/v2.4/me?access_token={0}&fields=id,name,email", accessToken);

        //        HttpWebRequest request = new HttpWebRequest(new Uri(url));
        //        request.Method = "GET";
        //        request.Accept = "application/json";

        //        var task = request.GetResponseAsync();
        //        if (!task.Wait(3000))
        //            throw new Exception("wait failed");
        //        HttpWebResponse httpResponse = (HttpWebResponse)task.Result;
        //        string json;
        //        using (Stream responseStream = httpResponse.GetResponseStream())
        //        {
        //            json = new StreamReader(responseStream).ReadToEnd();
        //        }
        //        string str = json;
        //        return str;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Exception: " + ex;
        //    }
        //}
    }
}
