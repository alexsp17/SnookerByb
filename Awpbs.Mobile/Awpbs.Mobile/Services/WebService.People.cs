using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace Awpbs.Mobile
{
    public partial class WebService
    {
        public async Task<PersonFullWebModel> GetPersonByID(int athleteID)
        {
			string url = WebApiUrl + "People?athleteID=" + athleteID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                PersonFullWebModel person = JsonConvert.DeserializeObject<PersonFullWebModel>(json);
                return person;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<PersonBasicWebModel>> GetPeopleByID(List<int> athleteIDs)
        {
			if (athleteIDs == null || athleteIDs.Count == 0)
				return null;
			string str = "";
			foreach (int id in athleteIDs)
			{
				if (str.Length > 0)
					str += ",";
				str += id.ToString();
			}
			string url = WebApiUrl + "People?athleteIDs=" + str;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<PersonBasicWebModel> people = JsonConvert.DeserializeObject<List<PersonBasicWebModel>>(json);
                return people;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<PersonBasicWebModel>> FindPeople(string nameQuery, Country country, int? metroID, bool friendsOnly, bool includeMyself = false)
        {
			string url = "";
			url = WebApiUrl + "People/Find?nameQuery=" + System.Net.WebUtility.UrlEncode(nameQuery);
			url += "&country=" + (country != null ? country.ThreeLetterCode : "");
			url += "&metroID=" + (metroID != null ? metroID.Value.ToString() : "0");
			url += "&friendsOnly=" + friendsOnly.ToString();
            url += "&includeMyself=" + includeMyself.ToString();
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<PersonBasicWebModel> people = JsonConvert.DeserializeObject<List<PersonBasicWebModel>>(json);
                return people;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<PersonBasicWebModel>> GetPeoplePlayedAtVenue(int venueID)
        {
			string url = WebApiUrl + "People/PlayedAtVenue?venueID=" + venueID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<PersonBasicWebModel> people = JsonConvert.DeserializeObject<List<PersonBasicWebModel>>(json);
                return people;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<PersonBasicWebModel>> GetFriends()
        {
			string url = WebApiUrl + "Friends";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<PersonBasicWebModel> people = JsonConvert.DeserializeObject<List<PersonBasicWebModel>>(json);
                return people;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<PersonBasicWebModel>> GetFriendsOfAthlete(int athleteID)
        {
			string url = WebApiUrl + "People/Friends?athleteID=" + athleteID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<PersonBasicWebModel> people = JsonConvert.DeserializeObject<List<PersonBasicWebModel>>(json);
                return people;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> RequestFriend(int athleteID)
        {
			string url = WebApiUrl + "Friends/RequestFriend?athleteID=" + athleteID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> Unfriend(int athleteID)
        {
			string url = WebApiUrl + "Friends/Unfriend?athleteID=" + athleteID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> AcceptFriendRequest(int friendshipID)
        {
			string url = WebApiUrl + "Friends/AcceptFriendRequest?friendshipID=" + friendshipID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> AcceptFriendRequest2(int athleteID)
        {
			string url = WebApiUrl + "Friends/AcceptFriendRequest2?athleteID=" + athleteID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> DeclineFriendRequest(int friendshipID)
        {
			string url = WebApiUrl + "Friends/DeclineFriendRequest?friendshipID=" + friendshipID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> WithdrawFriendRequest(int friendshipID)
        {
			string url = WebApiUrl + "Friends/WithdrawFriendRequest?friendshipID=" + friendshipID;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<List<FriendRequestWebModel>> GetFriendRequestsByMe()
        {
			string url = WebApiUrl + "Friends/GetFriendRequestsByMe";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<FriendRequestWebModel> friendRequests = JsonConvert.DeserializeObject<List<FriendRequestWebModel>>(json);
                return friendRequests;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<FriendRequestWebModel>> GetFriendRequestsToMe()
        {
			string url = WebApiUrl + "Friends/GetFriendRequestsToMe";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<FriendRequestWebModel> friendRequests = JsonConvert.DeserializeObject<List<FriendRequestWebModel>>(json);
                return friendRequests;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<ResultWebModel>> GetResults(int athleteID)
        {
			string url = WebApiUrl + "Results?athleteID=" + athleteID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<ResultWebModel> results = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                return results;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<ResultWebModel>> GetResultsAtVenue(int venueID)
        {
			string url = WebApiUrl + "Results/ResultsAtVenue?venueID=" + venueID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<ResultWebModel> results = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                return results;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<ResultWebModel>> GetResultsToConfirm()
        {
			string url = WebApiUrl + "Results/ResultsToConfirm";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<ResultWebModel> results = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                return results;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> ConfirmResult(int resultID, bool confirm)
        {
			string url = WebApiUrl + "Results/ConfirmResult?resultID=" + resultID + "&confirm=" + confirm;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<List<Score>> GetScores(int athleteID)
        {
			string url = WebApiUrl + "Scores?athleteID=" + athleteID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<Score> scores = JsonConvert.DeserializeObject<List<Score>>(json);
                return scores;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<Score>> GetScoresAtVenue(int venueID)
        {
			string url = WebApiUrl + "Scores/ScoresAtVenue?venueID=" + venueID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<Score> scores = JsonConvert.DeserializeObject<List<Score>>(json);
                return scores;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<Score>> GetScoresToConfirm()
        {
			string url = WebApiUrl + "Scores/ScoresToConfirm";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<Score> scores = JsonConvert.DeserializeObject<List<Score>>(json);
                return scores;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> ConfirmScore(int scoreID, bool confirm)
        {
			string url = WebApiUrl + "Scores/ConfirmScore?scoreID=" + scoreID + "&confirm=" + confirm;
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                return true;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }
    }
}
