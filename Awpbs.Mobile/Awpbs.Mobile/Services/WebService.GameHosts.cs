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
        public async Task<int?> NewGameHost(int venueID, DateTime when, EventTypeEnum eventType, int limitOnNumberOfPlayers, List<int> invitees, string comments)
		{
			string url = WebApiUrl + "GameHosts/NewGameHost2";
            try
            {
                NewGameHostWebModel2 model = new NewGameHostWebModel2()
                {
                    VenueID = venueID,
                    When = when.ToUniversalTime(),
                    When_InLocalTimeZone = when,
                    EventType = eventType,
                    LimitOnNumberOfPlayers = limitOnNumberOfPlayers,
                    Invitees = invitees,
                    Comments = comments,
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

		public async Task<bool> DeleteGameHost(int gameHostID)
		{
			string url = WebApiUrl + "GameHosts/DeleteGameHost?gameHostID=" + gameHostID;
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

        public async Task<bool> ChangeLimitOnNumberOfPlayers(int gameHostID, int limit)
        {
			string url = WebApiUrl + "GameHosts/ChangeLimitOnNumberOfPlayers?gameHostID=" + gameHostID + "&limit=" + limit;
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

        public async Task<int?> NewGameHostInvite(int gameHostID, int athleteID, DateTime when_InLocalTimeZone)
        {
			string url = WebApiUrl + "GameHosts/NewGameHostInvite";
            try
            {
                NewGameHostInviteWebModel model = new NewGameHostInviteWebModel()
                {
                    GameHostID = gameHostID,
                    AthleteID = athleteID,
                    When_InLocalTimeZone = when_InLocalTimeZone
                };
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
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

        public async Task<List<GameHostWebModel>> FindGameHosts(Location location, bool friendsOnly)
        {
			string url = "";
			url = WebApiUrl + "GameHosts/Find";
			url += "?friendsOnly=" + friendsOnly.ToString();
			if (location != null)
			{
				url += "&latitude=" + location.Latitude.ToString();
				url += "&longitude=" + location.Longitude.ToString();
			}
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<GameHostWebModel> gameHosts = JsonConvert.DeserializeObject<List<GameHostWebModel>>(json);
                foreach (var gameHost in gameHosts)
                    gameHost.When = System.TimeZone.CurrentTimeZone.ToLocalTime(gameHost.When);
                return gameHosts;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<GameHostWebModel>> GetGameHostsAtVenue(int venueID, bool includePast)
        {
			string url = WebApiUrl + "GameHosts/GameHostsAtVenue?venueID=" + venueID + "&includePast=" + includePast.ToString();
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<GameHostWebModel> gameHosts = JsonConvert.DeserializeObject<List<GameHostWebModel>>(json);
                foreach (var gameHost in gameHosts)
                    gameHost.When = System.TimeZone.CurrentTimeZone.ToLocalTime(gameHost.When);
                return gameHosts;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<GameHostWebModel>> GetMyGameHosts(bool includePast)
        {
			string url = WebApiUrl + "GameHosts/MyGameHosts?includePast=" + includePast.ToString ();
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<GameHostWebModel> gameHosts = JsonConvert.DeserializeObject<List<GameHostWebModel>>(json);
                foreach  (var gameHost in gameHosts)
                    gameHost.When = System.TimeZone.CurrentTimeZone.ToLocalTime(gameHost.When);
                return gameHosts;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<GameHostWebModel> GetGameHost(int gameHostID)
        {
			string url = WebApiUrl + "GameHosts?id=" + gameHostID;
			
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                GameHostWebModel gameHost = JsonConvert.DeserializeObject<GameHostWebModel>(json);
                gameHost.When = System.TimeZone.CurrentTimeZone.ToLocalTime(gameHost.When);
                return gameHost;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> ApproveGameHostInvitee(int gameHostID, int inviteeID)
        {
			string url = WebApiUrl + "GameHosts/ApproveInvitee?gameHostID=" + gameHostID.ToString() + "&inviteeID=" + inviteeID;
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

        public async Task<bool> DeclineGameHostInvite(int gameHostID)
        {
			string url = WebApiUrl + "GameHosts/DeclineInvitation?gameHostID=" + gameHostID.ToString();
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

        public async Task<bool> AskToJoinGameHost(int gameHostID)
        {
			string url = WebApiUrl + "GameHosts/AskToJoinGameHost?gameHostID=" + gameHostID;
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

        public async Task<bool> SendMessage(int athleteID, string messageText, bool shareMyEmail)
        {
			string url = WebApiUrl + "Messages/Send?athleteID=" + athleteID + "&messageText=" + System.Net.WebUtility.UrlEncode(messageText) + "&shareMyEmail=" + shareMyEmail;
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
