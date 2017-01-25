using AVFoundation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile.iOS
{
    public class ScorePronouncer_iOS : IScorePronouncer
    {
        /// <summary>
        /// Returns a list of available voices
        /// </summary>
        public List<Voice> GetVoices()
        {
            try
            {
                var voiceObjs = AVSpeechSynthesisVoice.GetSpeechVoices();

                List<Voice> voices = new List<Voice>();
                foreach (var voiceObj in voiceObjs)
                {
                    voices.Add(new Voice()
                    {
                        Language = voiceObj.Language,
                        Name = "Default iOS"
                    });
                }
                voices = voices.OrderBy(i => i.Language).ToList();

                return voices;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Pronounces the provided text
        /// </summary>
        public bool Pronounce(string textToPronounce, string voiceName, double? rateMult, double? pitchMult)
        {
            try
            {
                var speechUtterance = new AVSpeechUtterance(textToPronounce);

                // pick the voice
                Voice voice = null;
                if (string.IsNullOrEmpty(voiceName) == false)
                    voice = GetVoices().Where(i => i.FullName == voiceName).FirstOrDefault();
                if (voice != null)
                {
                    var voices = AVSpeechSynthesisVoice.GetSpeechVoices().ToList();
                    var voiceObj = voices.Where(i => i.Language == voice.Language).FirstOrDefault();
                    speechUtterance.Voice = voiceObj;
                }

                // rate
                double rate = rateMult != null ? rateMult.Value : UserPreferences.DefaultVoiceRate;
                if (Config.IsIOS && Config.OSVersionMajor < 9)
                    rate *= 0.3;

                // configure
                speechUtterance.Volume = 1.0f;
                speechUtterance.Rate = (float)rate;
                speechUtterance.PitchMultiplier = (float)(pitchMult != null ? pitchMult.Value : UserPreferences.DefaultVoicePitch);

                // pronounce
                var speechSynthesizer = new AVSpeechSynthesizer();
                speechSynthesizer.SpeakUtterance(speechUtterance);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
