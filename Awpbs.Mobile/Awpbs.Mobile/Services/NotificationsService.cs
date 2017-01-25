using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class NotificationsData
    {
        public bool InternetIssues { get; set; }

        public int NotificationsCount
        {
            get
            {
                int count = 0;
                count += FriendRequestsToMe != null ? FriendRequestsToMe.Count : 0;
                //count += FriendRequestsByMe != null ? FriendRequestsByMe.Count : 0;
                count += ResultsToConfirm != null ? ResultsToConfirm.Count : 0;
                count += ScoresToConfirm != null ? ScoresToConfirm.Count : 0;
                count += MyResultsToAccept != null ? MyResultsToAccept.Count : 0;
                return count;
            }
        }

        public List<FriendRequestWebModel> FriendRequestsToMe { get; set; }
        public List<FriendRequestWebModel> FriendRequestsByMe { get; set; }
        public List<ResultWebModel> ResultsToConfirm { get; set; }
        public List<Score> ScoresToConfirm { get; set; }
        public List<ResultWebModel> MyResultsToAccept { get; set; }
    }

    public class NotificationsService
    {
        protected event EventHandler Loaded;
        public NotificationsData Data { get; private set; }

        WebService webService;
        DateTime timeTaskStarted;
        Task task;

        public NotificationsService(WebService webService)
        {
            this.webService = webService;
        }

        public void SubscribeToLoadedEvent(EventHandler loaded)
        {
            if (Loaded != null)
            {
                Delegate[] clientList = Loaded.GetInvocationList();
                foreach (var d in clientList)
                    Loaded -= (d as EventHandler);
            }
            Loaded += loaded;
        }

		public void CheckForNotificationsIfNecessary(bool lazy = false)
        {
            if (task != null && task.IsCompleted == false)
                return;
			if ((DateTime.Now - timeTaskStarted).TotalSeconds < (lazy ? 40 : 10))
                return;

            timeTaskStarted = DateTime.Now;
            task = Task.Run(async () => { await checkForNotifications(); });
        }

        public void FireLoadedEvent()
        {
            if (Loaded != null)
                this.Loaded(this, EventArgs.Empty);
        }

        public void CheckForNotifications(EventHandler completed)
        {
            Task.Run(async () =>
            {
                await checkForNotifications();
                Device.BeginInvokeOnMainThread(() =>
                {
                    completed(this, EventArgs.Empty);
                });
            });
        }

        async Task checkForNotifications()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;

            try
            {
                var friendRequestsToMe = await App.WebService.GetFriendRequestsToMe();
                var friendRequestsByMe = await App.WebService.GetFriendRequestsByMe();
                var resultsToConfirm = await App.WebService.GetResultsToConfirm();
                var scoresToConfirm = await App.WebService.GetScoresToConfirm();
                var myResultsToAccept = await App.WebService.GetResultsNotYetAcceptedByMe();

                if (friendRequestsToMe != null)
                    friendRequestsToMe = (from i in friendRequestsToMe
                                          orderby i.PersonName
                                          select i).ToList();
                if (friendRequestsByMe != null)
                    friendRequestsByMe = (from i in friendRequestsByMe
                                          orderby i.PersonName
                                          select i).ToList();
                if (resultsToConfirm != null)
                    resultsToConfirm = (from i in resultsToConfirm
                                        orderby i.Date descending
                                        select i).ToList();
                if (scoresToConfirm != null)
                    scoresToConfirm = (from i in scoresToConfirm
                                       orderby i.Date descending
                                       select i).ToList();
                if (myResultsToAccept != null)
                    myResultsToAccept = (from i in myResultsToAccept
                                         orderby i.Date descending
                                         select i).ToList();

                int notificationsCount = 0;
                if (friendRequestsToMe != null)
                    notificationsCount += friendRequestsToMe.Count;
                if (resultsToConfirm != null)
                    notificationsCount += resultsToConfirm.Count;
                if (scoresToConfirm != null)
                    notificationsCount += scoresToConfirm.Count;
                if (myResultsToAccept != null)
                    notificationsCount += myResultsToAccept.Count;

                this.Data = new NotificationsData()
                {
                    InternetIssues = friendRequestsToMe == null,
                    FriendRequestsToMe = friendRequestsToMe,
                    FriendRequestsByMe = friendRequestsByMe,
                    ResultsToConfirm = resultsToConfirm,
                    ScoresToConfirm = scoresToConfirm,
                    MyResultsToAccept = myResultsToAccept
                };

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Loaded != null)
                        this.Loaded(this, EventArgs.Empty);
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
