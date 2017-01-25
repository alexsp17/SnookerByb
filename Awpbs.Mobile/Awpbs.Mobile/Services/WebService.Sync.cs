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
        public async Task<Athlete> SyncMyAthlete(Athlete athlete)
        {
			string url = WebApiUrl + "Sync/SyncMyAthlete";
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, athlete, true);
                var modelResponse = JsonConvert.DeserializeObject<Athlete>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<Result>> SyncResults(List<Result> resultsOnMobile)
        {
			string url = WebApiUrl + "Sync/SyncResults";
            try
            {
				var model = (from i in resultsOnMobile
					         select ResultWebModel.FromResult(i)).ToList();
				string json = await this.sendPostRequestAndReceiveResponse(url, model, true);
				var modelResponse = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                List<Result> syncedResults = (from i in modelResponse
                                              select i.ToResult()).ToList();
                return syncedResults;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<Score>> SyncScores(List<Score> scoresOnMobile)
        {
			string url = WebApiUrl + "Sync/SyncScores";
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, scoresOnMobile, true);
                var modelResponse = JsonConvert.DeserializeObject<List<Score>>(json);
                List<Score> syncedScores = (from i in modelResponse
                                            select i).ToList();
                return syncedScores;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool?> AddResultNotYetAcceptedByAthlete(ResultWebModel result)
        {
			string url = WebApiUrl + "Results/AddResultNotYetAcceptedByAthlete";
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, true);
                var modelResponse = JsonConvert.DeserializeObject<bool>(json);
                return modelResponse;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> AcceptResultNotYetAcceptedByMe(int resultID, bool accept)
        {
			string url = WebApiUrl + "Results/AcceptResultNotYetAcceptedByMe?resultID=" + resultID + "&accept=" + accept.ToString();
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

        public async Task<List<ResultWebModel>> GetResultsNotYetAcceptedByMe()
        {
			string url = WebApiUrl + "Results/ResultsNotYetAcceptedByMe";
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<ResultWebModel> list = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                return list;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<Score>> PersistScoresFVO(List<Score> scores)
        {
			string url = WebApiUrl + "Sync/PersistScoresFVO";
            try
            {
                var modelRequest = scores;
				string json = await this.sendPostRequestAndReceiveResponse(url, modelRequest, true);
                var modelResponse = JsonConvert.DeserializeObject<List<Score>>(json);
                List<Score> syncedScores = (from i in modelResponse
                                            select i).ToList();
                return syncedScores;
            }
            catch (Exception exc)
            {
                LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<ResultWebModel>> PersistResultsFVO(List<ResultWebModel> results)
        {
			string url = WebApiUrl + "Sync/PersistResultsFVO";
            try
            {
                var modelRequest = results;
				string json = await this.sendPostRequestAndReceiveResponse(url, modelRequest, true);
                var modelResponse = JsonConvert.DeserializeObject<List<ResultWebModel>>(json);
                return modelResponse;
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
