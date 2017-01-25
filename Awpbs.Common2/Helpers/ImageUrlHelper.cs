using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public static class ImageUrlHelper
    {
        public static string MakeUrlForOriginalImage(string url)
        {
            int index = url.LastIndexOf('/');

            string transformation = "";
            string modifiedUrl = url.Substring(0, index) + transformation + url.Substring(index, url.Length - index);

            return modifiedUrl;
        }

        public static string MakeUrlForMobileProfile(string url, BackgroundEnum background)
        {
            int index = url.LastIndexOf('/');

            string rgb;
            string border;

            switch (background)
            {
                case BackgroundEnum.Black:
                    rgb = "000000";
                    border = ",bo_4px_solid_white";
                    break;
                case BackgroundEnum.White:
                    rgb = "ffffff";
                    border = "";
                    break;
                default:
                    rgb = "252424";
                    border = ",bo_4px_solid_white";
                    break;
            }

            string transformation = "/w_180,h_180,c_thumb,g_face,r_90,b_rgb:" + rgb + border;
            string modifiedUrl = url.Substring(0, index) + transformation + url.Substring(index, url.Length - index);

            return modifiedUrl;
        }

        public static string MakeUrlForWebProfile(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "/images/personWhiteBackground.png";

            return MakeUrlForMobileProfile(url, BackgroundEnum.White);
        }
    }
}
