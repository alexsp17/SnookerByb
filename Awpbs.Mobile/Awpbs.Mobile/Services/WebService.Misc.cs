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
        public async Task<BybApiAboutWebModel> About()
        {
			string url = WebApiUrl + "About";
            try
            {
				string responseJson = await this.sendGetRequestAndReceiveResponse(url, true);
				BybApiAboutWebModel model = JsonConvert.DeserializeObject<BybApiAboutWebModel>(responseJson);

                return model;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<Athlete> GetMyAthlete()
        {
			string url = WebApiUrl + "MyAthlete";
            try
            {
				string responseJson = await this.sendGetRequestAndReceiveResponse(url, true);
				Athlete athlete = JsonConvert.DeserializeObject<Athlete>(responseJson);
                return athlete;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<string> UseFacebookPicture(bool use)
        {
			string url = WebApiUrl + "MyAthlete/UseFacebookPicture?use=" + use.ToString();
            try
            {
				string responseJson = await this.sendPostRequestAndReceiveResponse(url, true);
				string pictureUrl = JsonConvert.DeserializeObject<string>(responseJson);
                return pictureUrl;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<string> UploadPicture(Stream imageStream)
        {
			string url = WebApiUrl + "MyAthlete/UploadPicture";
            try
            {
				string responseJson = await this.sendPostRequestAndReceiveResponse(url, imageStream, true);
				string pictureUrl = JsonConvert.DeserializeObject<string>(responseJson);
                return pictureUrl;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<MetroWebModel> GetMetro(int metroID)
        {
			string url = WebApiUrl + "Metros?id=" + metroID;
            try
            {
				string responseJson = await this.sendGetRequestAndReceiveResponse(url, true);
				MetroWebModel metro = JsonConvert.DeserializeObject<MetroWebModel>(responseJson);
                return metro;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<List<MetroWebModel>> GetMetros(string country)
        {
			string url = WebApiUrl + "Metros?country=" + country;
            try
            {
				string responseJson = await this.sendGetRequestAndReceiveResponse(url, true);
				List<MetroWebModel> metros = JsonConvert.DeserializeObject<List<MetroWebModel>>(responseJson);
                return metros;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<List<MetroWebModel>> GetMetrosNearby(Location location)
        {
			string url = WebApiUrl + "Metros/Closest?latitude=" + location.Latitude.ToString("F5") + "&longitude=" + location.Longitude.ToString("F5");
            try
            {
				string responseJson = await this.sendGetRequestAndReceiveResponse(url, true);
                List<MetroWebModel> metros = JsonConvert.DeserializeObject<List<MetroWebModel>>(responseJson);
                return metros;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<bool> RegisterDeviceToken(string token, bool isApple, bool isAndroid)
        {
			string url = WebApiUrl + "PushNotifications/RegisterDeviceToken";
            try
            {
                RegisterDeviceTokenWebModel model = new RegisterDeviceTokenWebModel()
                {
                    Token = token,
                    IsApple = isApple,
                    IsAndroid = isAndroid,
                    IsNotProduction = Config.IsProduction == false,
                };
				string responseJson = await this.sendPostRequestAndReceiveResponse(url, model, true);
				bool response = JsonConvert.DeserializeObject<bool>(responseJson);
                return response;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }

        public async Task<bool> SendDeviceInfo()
        {
			string url = WebApiUrl + "About/DeviceInfo";
            try
            {
				DeviceInfoWebModel model = new DeviceInfoWebModel()
				{
					Platform = (int)(Config.IsAndroid ? MobilePlatformEnum.Android : MobilePlatformEnum.IOS),
					OSVersion = Config.OSVersion ?? "unknown",
					OtherInfo = "app version: " + SnookerBybMobileVersions.Current.ToString(),
				};
				string responseJson = await sendPostRequestAndReceiveResponse(url, model, true);
				bool response = JsonConvert.DeserializeObject<bool>(responseJson);
                return response;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return false;
            }
        }
    }
}
