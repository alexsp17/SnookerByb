using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class Voice
    {
        public string Language { get; set; }
        public string Name { get; set; }

        public string FullName
        {
            get
            {
				if (Config.IsIOS)
					return Name + " - " + Language;
				else
					return Name;
            }
        }

        public override string ToString()
        {
            return this.FullName;
        }
    }

    public interface IScorePronouncer
    {
        List<Voice> GetVoices();

		bool Pronounce(string textToPronounce, string voiceName, double? rateMult, double? pitchMult);
    }
}
