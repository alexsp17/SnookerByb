using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public enum RegistrationStatusEnum
    {
        FirstStarted = 0,
        //Guest = 1,
        Registered = 2
    }

    public class LoginAndRegistrationLogic
    {
        Repository repository;
        WebService webservice;
        FacebookService facebookService;
        IKeyChain keyChain;

        public LoginAndRegistrationLogic(FacebookService facebookService, Repository repository, WebService webservice, IKeyChain keyChain)
        {
            this.facebookService = facebookService;
            if (this.facebookService != null)
            {
                this.facebookService.LoginFailed += (s,e) => { Device.BeginInvokeOnMainThread(facebookService_LoginFailed); };
                this.facebookService.LoginSuccessful += (s,e) => { Device.BeginInvokeOnMainThread(facebookService_LoginSuccessful); };
            }

            this.repository = repository;
            this.webservice = webservice;
            this.keyChain = keyChain;

            this.webservice.AccessTokenExpired += webservice_AccessTokenExpired;
        }

        public RegistrationStatusEnum RegistrationStatus
        {
            get
            {
                int athleteID = repository.GetMyAthleteID();
                if (athleteID > 0)
                    return RegistrationStatusEnum.Registered;
                return RegistrationStatusEnum.FirstStarted;
            }
        }

        /// <summary>
        /// Forgets about the registration and goes to the "First Started" mode
        /// </summary>
        public void Unregister(bool silent)
        {
            try
            {
                keyChain.AccessToken = null;
                repository.SaveSecurity(0, false, DateTimeHelper.GetUtcNow());
                repository.DeleteAllData();
            }
            catch (Exception exc)
            {
                if (silent == false)
                    App.Navigator.DisplayAlertError("Failed to unregister: " + exc.Message);
            }

            if (silent == false)
                App.Navigator.DisplayAlertRegular("Please close the app now, and then start it again.");
        }

        public void StartSetup()
        {
            App.Navigator.OpenLoginPage();
        }

        public void StartSetupNewUser()
        {
            var page = new SetupNewUserPage();
            App.Navigator.NavPage.PushAsync(page);
        }

        public async void StartSetupNewUserRegisterWithEmail()
        {
            Athlete myAthleteOld = repository.GetMyAthlete();
            if (myAthleteOld != null && myAthleteOld.AthleteID > 0)
            {
                if (await askPermissionToLoginAsDifferentUser() != true)
                    return;
            }

			// ask for the e-mail
            var page = new SetupNewUserEmailPage();
            await App.Navigator.NavPage.PushAsync(page);
            page.ClickedRegister += (s, e) =>
            {
				string email = page.EMail;
				
				// ask for the PIN
				this.AskUserToEnterPin(
					false, false,
					async (string pin) =>
					{
						// pin is entered, proceed with registration
						await this.register(email, pin);
					},
					() => 
					{ 
						// canceled - no need to do anything
					});
            };
        }

		public void AskUserToEnterPin(bool asModal, bool isBlack, Action<string> onDone, Action onCanceled)
		{
			var pagePin = new EnterPinPage(isBlack)
			{
				TheTitle = "Set Your PIN",
				IsLabelBelowVisible = true
			};
			pagePin.UserClickedCancel += async () =>
			{
                if (asModal)
                    await App.Navigator.NavPage.Navigation.PopModalAsync();
                else
                    await App.Navigator.NavPage.PopAsync();
                onCanceled();
            };
			pagePin.UserEnteredPin += () =>
			{
				// validate PIN
				var pagePin2 = new EnterPinPage(isBlack)
				{
					TheTitle = "Re-Enter Your PIN",
					IsLabelBelowVisible = false
				};
				pagePin2.UserClickedCancel += async () =>
				{
					// close both pages
					if (asModal)
					{
						await App.Navigator.NavPage.Navigation.PopModalAsync();
						await App.Navigator.NavPage.Navigation.PopModalAsync();
					}
					else
					{
						await App.Navigator.NavPage.PopAsync();
						await App.Navigator.NavPage.PopAsync();
					}
					onCanceled();
				};
				pagePin2.UserEnteredPin += async () =>
				{
					// are the pins the same?
					string pin = pagePin.Pin;
					if (pagePin2.Pin != pin)
					{
						pagePin.ClearPin();
						if (asModal)
							await App.Navigator.NavPage.Navigation.PopModalAsync();
						else
							await App.Navigator.NavPage.PopAsync();
						await App.Navigator.NavPage.DisplayAlert("Byb", "PINs are different. Please re-enter the PIN.", "OK");
						return;
					}

					// close both pages
					if (asModal)
					{
						await App.Navigator.NavPage.Navigation.PopModalAsync();
						await App.Navigator.NavPage.Navigation.PopModalAsync();
					}
					else
					{
						await App.Navigator.NavPage.PopAsync();
						await App.Navigator.NavPage.PopAsync();
					}

					onDone(pin);
				};
				if (asModal)
					App.Navigator.NavPage.Navigation.PushModalAsync(pagePin2);
				else
					App.Navigator.NavPage.PushAsync(pagePin2);
			};
			if (asModal)
				App.Navigator.NavPage.Navigation.PushModalAsync (pagePin);
			else
				App.Navigator.NavPage.PushAsync(pagePin);
		}

		private async Task register(string email, string pin)
		{
			await this.openWaitPage();

			try
			{
				string password = pin;
				
				bool ok = await webservice.Register(email, password, null, "", null);
				//throw new Exception("test");
				if (ok == false)
				{
					await this.closeWaitPage();
					if (webservice.IsLastExceptionDueToInternetIssues)
						App.Navigator.DisplayAlertError("Couldn't register. Internet issues. Are you connected to the Internet?");
					else
						App.Navigator.DisplayAlertError("Couldn't register. Could it be that you already have a Byb account under this email address?");
					return;
				}

				if (!await webservice.Login(email, password, null))
					throw new Exception("Registered, but failed to login");

				var myAthlete = await webservice.GetMyAthlete();
				if (myAthlete == null)
					throw new Exception("Failed to complete registration operation");

				repository.DeleteAllData();
				repository.SaveSecurity(//accessToken, 
					myAthlete.AthleteID, false, myAthlete.TimeCreated);
				repository.UpdateAthlete(myAthlete);

				keyChain.Add(email, password);

				await registerDeviceTokenIfNecessary();
			}
			catch (Exception exc)
			{
				this.Unregister(true);
				App.Navigator.DisplayAlertError("Error while registering: " + exc.Message);
			}

			await this.closeWaitPage();

			if (RegistrationStatus != RegistrationStatusEnum.Registered)
			{
				StartSetup();
			}
			else
			{
				App.Navigator.OpenProfileEditPage(false, false);
			}
		}

        public void StartSetupExistingUser()
        {
            var page = new SetupExistingUserPage();
            App.Navigator.NavPage.PushAsync(page);

            page.ClickedLogin += async (s, e) =>
            {
                string email = page.EMail;
                string password = page.Password;

                await this.openWaitPage();

                Athlete myAthlete = null;
                Athlete myAthleteOld = repository.GetMyAthlete();

                try
                {
                    if (!await webservice.Login(email, password, null))
                    {
                        await this.closeWaitPage();
						if (webservice.IsLastExceptionDueToInternetIssues)
							App.Navigator.DisplayAlertError("Coudn't login. Internet issues. Are you connected to the internet?");
						else
                        	App.Navigator.DisplayAlertError("Couldn't login. Invalid user name / password?");
                        return;
                    }

                    myAthlete = await webservice.GetMyAthlete();
                    if (myAthlete == null)
                        throw new Exception("Failed to complete logging in");

                    if (myAthleteOld != null && myAthleteOld.AthleteID > 0)
                    {
                        if (myAthleteOld.AthleteID != myAthlete.AthleteID)
                        {
                            await this.closeWaitPage();

                            if (await askPermissionToLoginAsDifferentUser() != true)
                            {
                                keyChain.AccessToken = null;
                                //webservice.accessToken = null;
                                return;
                            }
                            
                            repository.DeleteAllData();
                        }
                    }

                    repository.SaveSecurity(//accessToken, 
                        myAthlete.AthleteID, false, myAthlete.TimeCreated);
                    repository.UpdateAthlete(myAthlete);

                    keyChain.Add(email, password);

                    await registerDeviceTokenIfNecessary();
                }
                catch (Exception exc)
                {
                    App.Navigator.DisplayAlertError("Failed to login. Error while registering: " + exc.Message);
                }

                if (RegistrationStatus == RegistrationStatusEnum.Registered)
                {
                    App.Navigator.OpenMainPage();
                }
                else
                {
                    this.StartSetup();
                }
            };
        }

        async Task<bool> askPermissionToLoginAsDifferentUser()
        {
            string strOk = "Ok to delete local data";
            string answer = await App.Navigator.NavPage.DisplayActionSheet(
                "You were previously logged in under a different account. To login with the new account all local data has to be deleted.", "Cancel", null, strOk);
            bool ok = answer == strOk;
            return ok;
        }

        private PleaseWaitPage waitPage = null;

        protected async Task openWaitPage()
        {
            if (this.waitPage != null)
                return;
            this.waitPage = new PleaseWaitPage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(this.waitPage);
        }

        protected async Task closeWaitPage()
        {
            if (waitPage == null)
                return;
            await App.Navigator.NavPage.Navigation.PopModalAsync(true);
            this.waitPage = null;
        }

        protected void updateWaitPageStatus(string status)
        {
            if (waitPage == null)
                return;
            waitPage.StatusText = status;
        }

        public async void StartLoginFromFacebook()
        {
            if (facebookService == null)
            {
                App.Navigator.DisplayAlertError("Error. facebookService is null.");
                return;
            }

            if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                App.Navigator.DisplayAlertRegular("Facebook login doesn't work in this beta yet.");
                return;
            }

            await openWaitPage();
            if (!facebookService.StartLogin())
            {
                await closeWaitPage();
                App.Navigator.DisplayAlertError("Error. Facebook login is unavailable.");
                return;
            }
        }

        async void facebookService_LoginSuccessful()
        {
            //if (RegistrationStatus == RegistrationStatusEnum.Registered)
            //{
            //    // no need to do anything
            //    return;
            //}

            bool accountAlreadyExisted = false;
            Athlete myAthleteOld = repository.GetMyAthlete();

            try
            {
                // load info about the user from facebook
                updateWaitPageStatus("Connecting with Facebook.");
                BasicFacebookProfileInfo facebookProfile = await facebookService.GetBasicProfileInfo();
                string athleteName = facebookProfile.Name;
                string realEmail = facebookProfile.EMail; // we don't need this for authentication. the server will get this from facebook's access token, if needed
                string facebookAccessToken = facebookService.FacebookAccessToken;

                keyChain.Add("facebook", facebookAccessToken);

                // build user name and password to login to Byb
                string loginEmail = LoginHelper.BuildLoginEmailFromFacebookId(facebookProfile.Id);
                string password = LoginHelper.BuildLoginPasswordFromFacebookId(facebookProfile.Id);

                // try logging in
                updateWaitPageStatus("Connecting with " + Config.WebsiteName);
                bool loggedIn = await this.webservice.Login(loginEmail, password, facebookAccessToken);
                if (loggedIn)
                {
                    // the user might already have an account
                    accountAlreadyExisted = true;
                }
                else
                {
                    if (myAthleteOld != null && myAthleteOld.AthleteID > 0)
                        if (await this.askPermissionToLoginAsDifferentUser() != true)
                            throw new Exception("user denied to login as a different user");

                    // register
                    accountAlreadyExisted = false;
                    if (!await this.webservice.Register(loginEmail, password, facebookProfile.Id, athleteName, facebookAccessToken))
                        throw new Exception("Failed to register");
                    loggedIn = await webservice.Login(loginEmail, password, facebookAccessToken);
                    if (loggedIn)
                        repository.DeleteAllData();
                }
                if (loggedIn == false)
                    throw new Exception("Failed to login");

                // load athlete data from the web
                updateWaitPageStatus("Loading info from " + Config.WebsiteName);
                var athlete = await webservice.GetMyAthlete();
                if (athlete == null)
                    throw new Exception("Failed to complete logging in");
                if (accountAlreadyExisted == false)
                {
                    if (string.IsNullOrEmpty(athleteName) == false)
                        athlete.Name = athleteName;
                }
                else
                {
                    if (myAthleteOld != null && myAthleteOld.AthleteID > 0 && myAthleteOld.AthleteID != athlete.AthleteID)
                    {
                        if (await this.askPermissionToLoginAsDifferentUser() != true)
                            throw new Exception("user denied to login as a different user");
                        repository.DeleteAllData();
                    }
                }

                // save to DB
                repository.SaveSecurity(//accessToken, 
                    athlete.AthleteID, false, athlete.TimeCreated);
                repository.UpdateAthlete(athlete);

                keyChain.Add(loginEmail, password);

                await registerDeviceTokenIfNecessary();

                // done
                if (accountAlreadyExisted)
                {
                    // note: wait page will close along with the current navigation page
                    App.Navigator.OpenMainPage();
                    if (Config.App != MobileAppEnum.SnookerForVenues)
                    {
                        await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Unknown, true);
                        App.Sync.StartAsync();
                    }
                }
                else
                {
					this.AskUserToEnterPin(
						true, false,
						(string pin) =>
						{
							// pin is entered, proceed with registration
							App.Navigator.OpenProfileEditPage(false, false);
						},
						() =>
						{
							// pin canceled. still proceed
							App.Navigator.OpenProfileEditPage(false, false);
						});
                }
                return;
            }
            catch (Exception)
            {
                //int asdf = 15;
            }

            await this.closeWaitPage();
            App.Navigator.DisplayAlertError("Failed to register. Internet issues?");
        }

        async void facebookService_LoginFailed()
        {
            await this.closeWaitPage();
            this.Unregister(true);
            App.Navigator.DisplayAlertError("Login to Facebook was canceled.");
        }

        ReloginPage reloginPage = null;
        DateTime? timeLastOpenedReloginPage = null;

        public async Task ShowReloginPage()
        {
            if (this.reloginPage != null)
                return; // the reloginPage is already open
            if (timeLastOpenedReloginPage != null && (DateTime.UtcNow - timeLastOpenedReloginPage.Value).TotalMinutes < 1)
                return; // already showed the reloginPage recently

            timeLastOpenedReloginPage = DateTime.UtcNow;

            this.reloginPage = new ReloginPage("You have been logged out from snookerbyb.com. Please re-login.");
            this.reloginPage.UserWantsToCancel += (s1, e1) =>
            {
                this.reloginPage = null;
                App.Navigator.OpenMainPage();
            };
            App.Navigator.OpenReloginPage(this.reloginPage);

            this.reloginPage.ShowOtherOptions = false;
            this.reloginPage.ShowAutoRelogin(true);

            await Task.Delay(1000);
            if (await this.tryToRelogin() == true)
            {
                // was able to re-login automatically
                this.reloginPage = null;
                App.Navigator.OpenMainPage();
                return;
            }
            else
            {
                // ask the user to re-login
                this.reloginPage.ShowOtherOptions = true;
                this.reloginPage.ShowAutoRelogin(false);
            }
        }

        private void webservice_AccessTokenExpired(object sender, EventArgs e)
        {
            var athlete = repository.GetMyAthlete();
            if (athlete == null || athlete.AthleteID == 0)
                return; // Has not been logged in yet. This could be happening while logging in. Don't do anything.
            if ((DateTime.UtcNow - App.TimeStarted).TotalSeconds < 5)
                return; // avoid showing this too quickly after the app starts, the modal dialog might not be able to show up

            Device.BeginInvokeOnMainThread(async () =>
            {
                await ShowReloginPage();
            });
        }

        async Task<bool> tryToRelogin()
        {
            try
            {
                var athlete = repository.GetMyAthlete();
                if (athlete == null || athlete.AthleteID <= 0)
                    return false;

                if (athlete.HasFacebookId)
                {
                    string facebookAccessToken = facebookService.FacebookAccessToken;
                    if (string.IsNullOrEmpty(facebookAccessToken))
                        facebookAccessToken = keyChain.Get("facebook");
                    if (string.IsNullOrEmpty(facebookAccessToken))
                        return false;

                    // build user name and password to login to Byb
                    string loginEmail = LoginHelper.BuildLoginEmailFromFacebookId(athlete.FacebookId);
                    string password = LoginHelper.BuildLoginPasswordFromFacebookId(athlete.FacebookId);

                    // try logging in
                    updateWaitPageStatus("Connecting with " + Config.WebsiteName);
                    if (!await this.webservice.Login(loginEmail, password, facebookAccessToken))
                        return false;
                }
                else
                {
                    string password = keyChain.Get(athlete.UserName);
                    if (string.IsNullOrEmpty(password))
                        return false;

                    if (!await webservice.Login(athlete.UserName, password, null))
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        async Task registerDeviceTokenIfNecessary()
        {
            try
            {
                System.Threading.Thread.Sleep(1000); // otherwise, the webservice call failes sometimes

                string deviceToken = keyChain.DeviceTokenToRegisterWhenLoggedIn;
                if (string.IsNullOrEmpty(deviceToken))
                    return;

                // register the device token
                if (await App.WebService.RegisterDeviceToken(deviceToken, Config.IsIOS, Config.IsAndroid) == true)
                {
                    keyChain.RegisteredDeviceToken = deviceToken;
                    keyChain.DeviceTokenToRegisterWhenLoggedIn = null;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
