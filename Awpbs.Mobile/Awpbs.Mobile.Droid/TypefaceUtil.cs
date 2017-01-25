using System;

using Android.Content;
using Android.Graphics;
using Android.Util;

using Java.Lang.Reflect;

namespace Awpbs.Mobile.Droid
{
    public class TypefaceUtil 
	{
		public static void SetDefaultFont(Context context, string staticTypefaceFieldName, string fontAssetName)
		{
			try 
			{
				Typeface regular = Typeface.CreateFromAsset(context.Assets, fontAssetName);
				ReplaceFont(staticTypefaceFieldName, regular);
			}
			catch (Exception exc)
			{
				var failureInfo = TraceHelper.ExceptionToString(exc);
				Console.WriteLine("TypefaceUtil::SetDefault(): Exception: " + failureInfo);

				return;
			}
		}

		protected static void ReplaceFont(string staticTypefaceFieldName, Typeface newTypeface)
		{
			try
			{
				Field staticField = ((Java.Lang.Object)(newTypeface)).Class.GetDeclaredField(staticTypefaceFieldName);
				staticField.Accessible = true;
				staticField.Set(null, newTypeface);
			}
			catch (System.Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
    }
}
