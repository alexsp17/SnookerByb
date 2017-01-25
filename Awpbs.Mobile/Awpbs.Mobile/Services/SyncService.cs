using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public enum SyncResultEnum
    {
        None = 0,
        Failed = 1,
        Ok = 2
    }

    public class SyncResult
    {
        public DateTime TimeStarted { get; set; }
        public DateTime TimeCompleted { get; set; }
        public SyncResultEnum Result { get; set; }

        public int ResultsCountBeforeSync { get; set; }
        public int ResultsCountAfterSync { get; set; }

        public int ScoresCountBeforeSync { get; set; }
        public int ScoresCountAfterSync { get; set; }

        public string Info { get; set; }
    }

    public class SyncStatus
    {
        public bool Completed { get; set; }
        public bool Error { get; set; }

        public int TotalCount { get; set; }
        public int Current { get; set; }
    }

    public abstract class SyncServiceBase
    {
        protected Repository repository;
        protected WebService webservice;

        public SyncServiceBase(Repository repository, WebService webservice)
        {
            this.repository = repository;
            this.webservice = webservice;

            this.SyncResults = new List<SyncResult>();
        }

        public List<SyncResult> SyncResults { get; private set; }

        public SyncResult LastSyncResults { get { return SyncResults.LastOrDefault(); } }

        public event EventHandler<SyncStatus> StatusChanged;

        public void StartAsync()
        {
            if (App.LoginAndRegistrationLogic.RegistrationStatus != RegistrationStatusEnum.Registered)
                return;

            Task.Run(async delegate
            {
				Device.BeginInvokeOnMainThread(() =>
				{
					if (this.StatusChanged != null)
						this.StatusChanged(this, new SyncStatus() { Completed = false, Current = 0, TotalCount = 0 });
				});

                SyncResult results = await sync();
				this.SyncResults.Add(results);

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (this.StatusChanged != null)
                        this.StatusChanged(this, new SyncStatus() { Error = results.Result == SyncResultEnum.Failed, Completed = true, Current = 0, TotalCount = 0 });
                });
            });
        }

        protected void onStatusChanged(bool completed, int current, int totalCount)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (this.StatusChanged != null)
                    this.StatusChanged(this, new SyncStatus() { Completed = completed, Current = current, TotalCount = totalCount });
            });
        }

        protected abstract Task<SyncResult> sync();
    }
}
