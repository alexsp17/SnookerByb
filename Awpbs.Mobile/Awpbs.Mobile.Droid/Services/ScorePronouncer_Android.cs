using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Java.Util;
using Android.Speech.Tts;
using Xamarin.Forms;
using Awpbs.Mobile.Droid;

namespace Awpbs.Mobile.Droid
{
	public class ScorePronouncer_Android : Java.Lang.Object, IScorePronouncer, TextToSpeech.IOnInitListener
	{
		private TextToSpeech textToSpeech;
        private string currentVoiceName;
        float currentSpeechRate;
        float currentSpeechPitch;
        List<Voice> androidVoices;
        bool voicesInitialized;

		public ScorePronouncer_Android()
		{
			this.init();
		}

		private void init()
		{
			var c = Forms.Context; 
			textToSpeech = new TextToSpeech (c, this);

            voicesInitialized = false;
            List<Voice> androidVoices = new List<Voice>();

            currentVoiceName = ""; // default voice
            setSpeechRate((float)UserPreferences.DefaultVoiceRate);
            setSpeechPitch((float)UserPreferences.DefaultVoicePitch);
		}
			
		#region IOnInitListener implementation 
		public void OnInit (OperationResult status)
		{
			if (status.Equals (OperationResult.Success)) 
            {
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
                Console.WriteLine("textToSpeech init: Language {0}", Java.Util.Locale.Default.DisplayLanguage);
			} 
            else 
            {
				System.Diagnostics.Debug.WriteLine ("was quiet");
			}
		}
		#endregion

		void ttsSpeak(string textToPronounce, bool interruptIfCurrentlySpeaking)
		{
			QueueMode speakQueueMode = QueueMode.Add;

			if (interruptIfCurrentlySpeaking) 
			{
				speakQueueMode = QueueMode.Flush;
			}

			if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop) 
			{
				textToSpeech.Speak(textToPronounce, speakQueueMode, null, null);
			}
			else
			{
				textToSpeech.Speak(textToPronounce, speakQueueMode, null);
			}
		}

        void setSpeechRate(float speechRate)
        {
            textToSpeech.SetSpeechRate(speechRate);
            currentSpeechRate = speechRate;
        }

        void setSpeechPitch(float speechPitch)
        {
            textToSpeech.SetPitch(speechPitch);
            currentSpeechPitch = speechPitch;
        }

        private void setVoice(string voiceName)
        {
            currentVoiceName = voiceName;

            try
            {
                // pick the voice
                Voice voice = null;
                if (string.IsNullOrEmpty(voiceName) == false)
                    voice = GetVoices().Where(i => i.FullName == voiceName).FirstOrDefault();
            
                if (voice != null)
                {
                    var localesAvailable = Java.Util.Locale.GetAvailableLocales();
                    var displayLanguage = localesAvailable.Where(i => i.DisplayName == voiceName).FirstOrDefault();
                    textToSpeech.SetLanguage(displayLanguage);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine ("Haven't set up voice, using default");
                }
            }
	        catch (Exception e)
		    {
                var failureInfo = TraceHelper.ExceptionToString(e);
                Console.WriteLine("setVoice(): " + failureInfo);

			    return;
		    }
        }

		public List<Voice> GetVoices()
		{
			try
			{
                // call this only once to get voices and save them.
                // The following calls, just return the previously saved voices
                if (voicesInitialized)
					return androidVoices;

				List<Voice> voices = new List<Voice>();

				var localesAvailable = Java.Util.Locale.GetAvailableLocales();
				foreach (var locale in localesAvailable)
				{
					try
					{
						var res = textToSpeech.IsLanguageAvailable(locale);

						switch (res)
						{
							case LanguageAvailableResult.Available:
							case LanguageAvailableResult.MissingData:  // MissingData actually seems to work ok (Spanish, German)
							//case LanguageAvailableResult.CountryAvailable:
							//case LanguageAvailableResult.CountryVarAvailable:

							// There is "English", there also a long list of English (Australia) ... English(Zimbabwe).
							// Just take one default locale per language for now
							if (locale.DisplayLanguage == locale.DisplayName)
							{
								voices.Add(new Voice()
									{
										Language = locale.DisplayLanguage,
										Name = locale.DisplayName
									});

								//System.Diagnostics.Debug.WriteLine ("DisplayName, avail: " + locale.DisplayName + " " + res.ToString());
							}
							break;
						}
					}
					catch (Exception)
					{
						continue;
					}
				}
				voices = voices.OrderBy(i => i.Language).ToList();
                
                androidVoices = voices;
                voicesInitialized = true;

				return androidVoices;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Pronounces the score
		/// </summary>
		public bool Pronounce(string textToPronounce, string voiceName, double? rateMult, double? pitchMult)
		{
			try
			{
                // if voice changed, set the new voice in tts
                if (voiceName != currentVoiceName)
					setVoice(voiceName);

				// reconfigure speech rate and pitch if needed        
                if (currentSpeechRate != (float)rateMult.Value)
					setSpeechRate((float)rateMult.Value);
                if (currentSpeechPitch != (float)pitchMult.Value)
					setSpeechPitch((float)pitchMult.Value);

				// pronounce
				ttsSpeak(textToPronounce, true);

		        // for debugging
                /*
			    System.Diagnostics.Debug.WriteLine ("rate " + speechRate);
			    System.Diagnostics.Debug.WriteLine (" pitch " + speechPitch);
			    System.Diagnostics.Debug.WriteLine ("spoke " + textToPronounce);
			    */

			    return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

	}
}

