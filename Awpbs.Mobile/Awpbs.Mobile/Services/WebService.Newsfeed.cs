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
        public async Task<int?> MakePost(string country, int metroID, string text)
        {
			string url = WebApiUrl + "Newsfeed/MakePost";
            try
            {
                NewPostWebModel model = new NewPostWebModel()
                {
                    Country = country,
                    MetroID = metroID,
                    Text = text
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model, true);
                int modelResponse = JsonConvert.DeserializeObject<int>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<int?> MakeComment(NewsfeedItemTypeEnum objectType, int objectID, string text)
        {
			string url = WebApiUrl + "Newsfeed/MakeComment";
            try
            {
                NewCommentWebModel model = new NewCommentWebModel()
                {
                    ObjectType = objectType,
                    ObjectID = objectID,
                    Text = text
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model, true);
                int modelResponse = JsonConvert.DeserializeObject<int>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> DeleteComment(int commentID)
        {
			string url = WebApiUrl + "Newsfeed/DeleteComment?commentID=" + commentID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                bool modelResponse = JsonConvert.DeserializeObject<bool>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> SetLike(NewsfeedItemTypeEnum objectType, int objectID, bool like)
        {
			string url = WebApiUrl + "Newsfeed/SetLike";
            try
            {
                SetLikeWebModel model = new SetLikeWebModel()
                {
                    ObjectType = objectType,
                    ObjectID = objectID,
                    Like = like
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, model, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<NewsfeedItemWebModel> GetNewsfeedItemFromCommentID(int commentID)
        {
			string url = WebApiUrl + "Newsfeed/SingleFromCommentID?commentID=" + commentID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                NewsfeedItemWebModel item = JsonConvert.DeserializeObject<NewsfeedItemWebModel>(json);
                item.Time = System.TimeZone.CurrentTimeZone.ToLocalTime(item.Time);
                return item;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<NewsfeedWebModel> GetNewsfeed(string country)
        {
			string url = WebApiUrl + "Newsfeed/Country?countryCode=" + country;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                NewsfeedWebModel newsfeed = JsonConvert.DeserializeObject<NewsfeedWebModel>(json);
                foreach (var item in newsfeed.Items)
                    item.Time = System.TimeZone.CurrentTimeZone.ToLocalTime(item.Time);
                return newsfeed;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<NewsfeedWebModel> GetNewsfeed(int metroID)
        {
			string url = WebApiUrl + "Newsfeed/Metro?id=" + metroID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                NewsfeedWebModel newsfeed = JsonConvert.DeserializeObject<NewsfeedWebModel>(json);
                foreach (var item in newsfeed.Items)
                    item.Time = System.TimeZone.CurrentTimeZone.ToLocalTime(item.Time);
                return newsfeed;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<CommentWebModel>> GetComments(NewsfeedItemTypeEnum objectType, int objectID)
        {
			string url = WebApiUrl + "Newsfeed/Comments?objectType=" + ((int)objectType).ToString() + "&objectID=" + objectID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<CommentWebModel> comments = JsonConvert.DeserializeObject<List<CommentWebModel>>(json);
                foreach (var item in comments)
                    item.Time = System.TimeZone.CurrentTimeZone.ToLocalTime(item.Time);
                return comments;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }
    }
}
