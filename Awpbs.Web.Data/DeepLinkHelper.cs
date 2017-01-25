using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace Awpbs.Web
{
    public class DeepLinkHelper
    {
        public class BranchLinkRequestResult
        {
            public string Url { get; set; }
        }

        public string BuildLinkToSync(int athleteID)
        {
            return "https://snookerbyb.com/sync/" + athleteID;
        }

        public string BuildLinkToAthlete(int athleteID)
        {
            return "https://snookerbyb.com/players/" + athleteID;
        }

        public string BuildLinkToGameHost(int gameHostID)
        {
            return "https://snookerbyb.com/invites/" + gameHostID;
        }

        public string BuildLinkToComment(int commentID)
        {
            return "https://snookerbyb.com/comments/" + commentID;
        }

        //public string BuildOpenBybLink()
        //{
        //    return "https://bnc.lt/a/key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe";
        //}

        public string BuildOpenBybLink_Sync(int athleteID)
        {
            return "https://bnc.lt/a/key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe?$deeplink_path=sync/" + athleteID;
        }

        public string BuildOpenBybLink_Athlete(int athleteID)
        {
            return "https://bnc.lt/a/key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe?$deeplink_path=athlete/" + athleteID;
        }

        public string BuildOpenBybLink_GameHost(int gameHostID)
        {
            return "https://bnc.lt/a/key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe?$deeplink_path=invite/" + gameHostID;
        }

        public string BuildOpenBybLink_Comment(int commentID)
        {
            return "https://bnc.lt/a/key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe?$deeplink_path=comments/" + commentID;
        }

        [Obsolete]
        public async Task<string> BuildLink_Athlete_Old(int athleteID)
        {
            try
            {
                // build a request
                var client = new HttpClient();
                var requestContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("branch_key", "key_live_iebJ0jOkqjzp4Co7nYiEAhlbEDolNjJe"),
                    new KeyValuePair<string, string>("data", "{ \"$deeplink_path\":\"athlete/" + athleteID + "\"}"),
                });

                // Get the response.
                HttpResponseMessage response = await client.PostAsync(
                    "https://api.branch.io/v1/url",
                    requestContent);
                HttpContent responseContent = response.Content;
                string strResponse;
                using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    strResponse = await reader.ReadToEndAsync();
                }

                // parse the response
                var result = JsonConvert.DeserializeObject<BranchLinkRequestResult>(strResponse);
                string url = result.Url;
                if (url == null)
                    return "";
                return url;
            }
            catch (Exception exc)
            {
                return "";
            }
        }
    }
}
