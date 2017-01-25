using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SyncServiceFVO : SyncServiceBase
    {
        public SyncServiceFVO(Repository repository, WebService webservice)
            : base(repository, webservice)
        {
        }

        protected override async Task<SyncResult> sync()
        {
            if (Config.App != MobileAppEnum.SnookerForVenues)
                throw new Exception("App = " + Config.App.ToString());

            SyncResult syncRes = new SyncResult();
            syncRes.TimeStarted = DateTime.Now;
            syncRes.Info = "test\r\n";

            try
            {
                /// scores
                /// 

                onStatusChanged(false, 1, 4);
                System.Threading.Thread.Sleep(500);

                // load scores from local db
                List<Score> localScores = App.Repository.GetScores(false);

                // send these scores to the web service
                List<Score> persistedScores = await App.WebService.PersistScoresFVO(localScores);

                onStatusChanged(false, 2, 4);
                System.Threading.Thread.Sleep(500);

                // set all persisted scores as 'deleted'
                foreach (var score in persistedScores)
                {
                    var localScore = localScores.Where(i => i.Guid == score.Guid).FirstOrDefault();
                    if (localScore == null)
                        continue; // this shouldn't be happening

                    App.Repository.SetIsDeletedOnScore(localScore.ScoreID, true);
                }

                onStatusChanged(false, 3, 4);
                System.Threading.Thread.Sleep(500);

                /// results
                /// 

                // load results from local db
                List<Result> localResults = App.Repository.GetResults(false);

                // send these results to the web service
                List<ResultWebModel> persistedResults = await App.WebService.PersistResultsFVO((from r in localResults
                                                                                                select ResultWebModel.FromResult(r)).ToList());
                onStatusChanged(false, 4, 4);
                System.Threading.Thread.Sleep(500);

                // set all persisted results as 'deleted'
                foreach (var result in persistedResults)
                {
                    var localResult = localResults.Where(i => i.Guid == result.Guid).FirstOrDefault();
                    if (localResults == null)
                        continue; // this shouldn't be happening

                    App.Repository.SetIsDeletedOnResult(localResult.ResultID, true);
                }

                syncRes.Result = SyncResultEnum.Ok;
                syncRes.TimeCompleted = DateTime.Now;
                return syncRes;
            }
            catch (Exception exc)
            {
                syncRes.Result = SyncResultEnum.Failed;
                syncRes.TimeCompleted = DateTime.Now;
                syncRes.Info += "Exception: " + TraceHelper.ExceptionToString(exc);
                return syncRes;
            }
        }
    }
}
