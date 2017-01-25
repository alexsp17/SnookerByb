using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class SyncServiceRegular : SyncServiceBase
    {
        static bool dbgSyncFlag = false;

        public SyncServiceRegular(Repository repository, WebService webservice)
            : base(repository, webservice)
        {
        }

        void dbgPrint(List<Result> results, string toPrint)
        {
            if (dbgSyncFlag)
            {
                Console.WriteLine(toPrint + " Count: " + results.Count);
                for (int idx=0; idx<results.Count; idx++)
                {
                    Console.WriteLine("Idx " + idx + ": " +
                        "Guid " + results[idx].Guid    + ", " +
                        "Date " + results[idx].Date    + ", " +
                        "Balls " + results[idx].Count2 + ", " +
                        "Points " + results[idx].Count + ", " +
                        "Details " + results[idx].Details1);
                }
            }
        }

        protected override async Task<SyncResult> sync()
        {
            if (Config.App != MobileAppEnum.Snooker)
                throw new Exception("App = " + Config.App.ToString());

            SyncResult syncRes = new SyncResult();
            syncRes.TimeStarted = DateTime.Now;
            syncRes.Info = "test\r\n";

            try
            {
                System.Threading.Thread.Sleep(100);

                /// Sync "My Athlete" record
                /// 

                var athlete = repository.GetMyAthlete();
                if (athlete.AthleteID == 0)
                {
                    syncRes.Result = SyncResultEnum.Failed;
                    syncRes.TimeCompleted = DateTime.Now;
                    return syncRes;
                }
                syncRes.Info += "AthleteID=" + athlete.AthleteID + "\r\n";
				Console.WriteLine("sync: athlete.TimeModified=" + athlete.TimeModified.ToShortDateString() + " - " + athlete.TimeModified.ToLongTimeString());

                var athleteFromWeb = await webservice.SyncMyAthlete(athlete);
                if (athleteFromWeb != null)
                {
                    syncRes.Info += "Synced athlete record\r\n";

                    // non-null means that the web returned updated version of Athlete record
                    athleteFromWeb.CopyTo(athlete, true);
                    athlete.TimeModified = athleteFromWeb.TimeModified;
                    repository.UpdateAthlete(athlete);
                }
                else
                    syncRes.Info += "Athlete record didn't change\r\n";

                /// Send all results of opponents
                /// 
                await this.sendAllResultsByOpponents();

                /// Sync "Results" and "Scores"
                /// 

                if (await this.syncResults(athlete, syncRes) == false)
                    return syncRes;
                if (await this.syncScores(syncRes) == false)
                    return syncRes;

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

        async Task<bool> sendAllResultsByOpponents()
        {
            try
            {
                var results = repository.GetResultsForOtherAthletes();

                foreach (var result in results)
                {
                    bool? ok = await webservice.AddResultNotYetAcceptedByAthlete(ResultWebModel.FromResult(result));
                    if (ok == null)
                        return false; // internet issues
                    repository.DeleteResult(result.ResultID);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        async Task<bool> syncResults(Athlete athlete, SyncResult syncRes)
        {
            var resultsOnMobile = repository.GetResults(athlete.AthleteID, true);
            syncRes.ResultsCountBeforeSync = resultsOnMobile.Count;

            syncRes.Info += "Starting sync";
            var syncedResults = await webservice.SyncResults(resultsOnMobile);
            if (syncedResults == null)
            {
                syncRes.Result = SyncResultEnum.Failed;
                syncRes.TimeCompleted = DateTime.Now;
                return false;
            }
            syncRes.ResultsCountAfterSync = syncedResults.Count;

            if (dbgSyncFlag)
            {
                dbgPrint(resultsOnMobile, "resultsOnMobile");
                dbgPrint(syncedResults, "syncedResults");
            }

            // any changes?
            bool hasChanges = false;
            if (resultsOnMobile.Count != syncedResults.Count)
            {
                hasChanges = true;
            }
            else
            {
                foreach (var result in syncedResults)
                    if ((from i in resultsOnMobile
                         where i.IsDifferent(result) == false
                         select i).Count() == 0)
                        hasChanges = true;
            }

            if (hasChanges == false)
            {
                syncRes.Result = SyncResultEnum.Ok;
                syncRes.TimeCompleted = DateTime.Now;
                return true;
            }

            repository.UpdateAllAthleteResults(athlete.AthleteID, syncedResults);

            if (dbgSyncFlag)
            {
                var resultsOnMobile2 = repository.GetResults(athlete.AthleteID, true);
                dbgPrint(resultsOnMobile2, "resultsOnMobile After update");
            }

            return true;
        }

        async Task<bool> syncScores(SyncResult syncRes)
        {
            var scoresOnMobile = repository.GetScores(true);

            // note: do not sync unfinished scores
            var scoresOnMobile_Unfinished = (from i in scoresOnMobile
                                             where i.IsUnfinished || i.AthleteBID == 0
                                             select i).ToList();
            var scoresOnMobile_Finished = (from i in scoresOnMobile
                                           where i.IsUnfinished == false && i.AthleteBID != 0
                                           select i).ToList();

            syncRes.ScoresCountBeforeSync = scoresOnMobile_Finished.Count;

            syncRes.Info += "Starting scores sync";
            var syncedScores = await webservice.SyncScores(scoresOnMobile_Finished);
            if (syncedScores == null)
            {
                syncRes.Result = SyncResultEnum.Failed;
                syncRes.TimeCompleted = DateTime.Now;
                return false;
            }
            syncRes.ScoresCountAfterSync = syncedScores.Count;

            // any changes?
            bool hasChanges = false;
            if (scoresOnMobile_Finished.Count != syncedScores.Count)
            {
                hasChanges = true;
            }
            else
            {
                foreach (var result in syncedScores)
                    if ((from i in scoresOnMobile_Finished
                         where i.IsDifferent(result, true) == false
                         select i).Count() == 0)
                        hasChanges = true;
            }

            if (hasChanges == false)
            {
                syncRes.Result = SyncResultEnum.Ok;
                syncRes.TimeCompleted = DateTime.Now;
                return true;
            }

            foreach (var score in scoresOnMobile_Unfinished)
                syncedScores.Add(score);

            repository.UpdateAllScores(syncedScores);
            return true;
        }
    }
}
