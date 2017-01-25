using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class ImagesService
    {
        public ImageSource GetImageSource(string picture, BackgroundEnum background = BackgroundEnum.White, bool originalSize = false)
        {
            if (string.IsNullOrEmpty(picture) == true || picture.ToLower().StartsWith("http") == false)
            {
                string file;
                if (background == BackgroundEnum.Background1)
                    file = "personBackground1.png";
                else if (background == BackgroundEnum.Black)
                    file = "personBlackBackground.png";
                else
                    file = "personWhiteBackground.png";
                return new FileImageSource() { File = file };
            }

            string url;
            if (originalSize)
                url = ImageUrlHelper.MakeUrlForOriginalImage(picture);
            else
                url = ImageUrlHelper.MakeUrlForMobileProfile(picture, background);
                
            return new UriImageSource() { Uri = new Uri(url) };
        }
    }
}
