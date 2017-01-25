using System;
using System.IO;

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
#endif

#if WINDOWS_PHONE
using Microsoft.Phone;
using System.Windows.Media.Imaging;
#endif

namespace Awpbs.Mobile
{
    public static class ImageResizer
    {
        static ImageResizer()
        {
        }

        public static System.IO.Stream ResizeImage(System.IO.Stream stream, float width)
        {
            byte[] source = new System.IO.BinaryReader(stream).ReadBytes((int)stream.Length);
            byte[] result = ResizeImage(source, width);
            if (result == null)
                return null;
            return new System.IO.MemoryStream(result);
        }

        public static byte[] ResizeImage(byte[] imageData, float width)
        {
#if __IOS__
            return ResizeImageIOS(imageData, width);
#endif
#if __ANDROID__
			return ResizeImageAndroid ( imageData, width);
#endif
#if WINDOWS_PHONE
			return ResizeImageWinPhone ( imageData, width, height );
#endif
        }


#if __IOS__
        public static byte[] ResizeImageIOS(byte[] imageData, float width)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            float originalWidth = (float)originalImage.Size.Width;
            float originalHeight = (float)originalImage.Size.Height;

            float newWidth = width;
            float newHeight = (float)(width * originalHeight / originalWidth);

            UIImage newImage = originalImage.Scale(new CGSize() { Width = newWidth, Height = newHeight });
            return newImage.AsPNG().ToArray();

            //try
            //{
            //    using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
            //        (int)newWidth, (int)newHeight, 8,
            //        (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB(),
            //        CGImageAlphaInfo.PremultipliedFirst))
            //    {
            //        context.ToImage()

            //        // draw the image
            //        float w = newWidth;
            //        float h = newHeight;
            //        //if (originalWidth < originalHeight)
            //        //{
            //        //    w = newHeight;
            //        //    h = newWidth;
            //        //}
            //        context.DrawImage(new RectangleF(0, 0, w, h), originalImage.CGImage);

            //        UIImageOrientation orientation = UIImageOrientation.
            //        if (originalWidth < originalHeight)

            //        UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(),);

            //        // save the image as a jpeg
            //        return resizedImage.AsJPEG().ToArray();
            //    }
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
#endif

#if __ANDROID__

		public static byte[] ResizeImageAndroid (byte[] imageData, float width)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);
            float height = (float)(width * originalImage.Height / originalImage.Width);

			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress (Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray ();
			}
		}

#endif

#if WINDOWS_PHONE

        public static byte[] ResizeImageWinPhone (byte[] imageData, float width)
        {
            byte[] resizedData;

            using (MemoryStream streamIn = new MemoryStream (imageData))
            {
                float height = (float)(width * imageData.Height / imageData.Width);

                WriteableBitmap bitmap = PictureDecoder.DecodeJpeg (streamIn, (int)width, (int)height);

                using (MemoryStream streamOut = new MemoryStream ())
                {
                    bitmap.SaveJpeg(streamOut, (int)width, (int)height, 0, 100);
                    resizedData = streamOut.ToArray();
                }
            }
            return resizedData;
        }
        
#endif

    }
}