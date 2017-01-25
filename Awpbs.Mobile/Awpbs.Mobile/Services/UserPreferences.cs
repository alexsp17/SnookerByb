using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Awpbs.Mobile
{
    public class UserPreferences
    {
        IFiles files;

        public UserPreferences()
        {
            this.files = App.Files;
        }

        public UserPreferences(IFiles files)
        {
            this.files = files;
        }

        public bool IsVoiceOn
        {
            get
            {
                return Get("IsVoiceOn") == "false" ? false : true;
            }
            set
            {
                Set("IsVoiceOn", value ? "true" : "false");
            }
        }

        public readonly static double MinVoiceRate = 0.2;
        public readonly static double MaxVoiceRate = Config.IsIOS ? 2.0 : 3.0;
        public readonly static double DefaultVoiceRate = Config.IsIOS ? 0.5 : 1.0;

        public double VoiceRate
        {
            get
            {
                double rate;
                if (!double.TryParse(Get("VoiceRate"), out rate))
                    return DefaultVoiceRate;
                return rate;
            }
            set
            {
                Set("VoiceRate", value.ToString());
            }
        }

        public const double MinVoicePitch = 0.2;
        public const double MaxVoicePitch = 3.0;
        public const double DefaultVoicePitch = 1.0;

        public double VoicePitch
        {
            get
            {
                double pitch;
                if (!double.TryParse(Get("VoicePitch"), out pitch))
                    return DefaultVoicePitch;
                return pitch;
            }
            set
            {
                Set("VoicePitch", value.ToString());
            }
        }

        public string Voice
        {
            get
            {
                string str = Get("Voice");
                if (str == null)
                    str = "";
                return str;
            }
            set
            {
                Set("Voice", value ?? "");
            }
        }

        public string Get(string name)
        {
            try
            {
                string fileContent = this.readFileContent();

                List<string> lines = fileContent.Split('\n').ToList();

                foreach (string line in lines)
                    if (line.StartsWith(name + "::"))
                    {
                        string val = line.Substring(name.Length + 2);
                        val = JsonConvert.DeserializeObject<string>(val);
                        return val;
                    }
                return "";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Set(string name, string val)
        {
            try
            {
                string fileContent = this.readFileContent();
                if (fileContent == null)
                    fileContent = "";

                List<string> lines = fileContent.Split('\n').ToList();
                lines.Insert(0, name + "::" + JsonConvert.SerializeObject(val));

                fileContent = "";
                for (int i = 0; i < lines.Count; ++i)
                {
                    string line = lines[i];
                    if (line == "")
                        continue;
                    if (i != 0 && line.StartsWith(name + "::"))
                        continue; // remove the old version for this "name"
                    fileContent += line;
                    if (i != lines.Count - 1)
                        fileContent += "\n";
                }

                updateFileContent(fileContent);
            }
            catch (Exception)
            {
            }
        }

        void updateFileContent(string fileContent)
        {
            string fileName = getFileName();
            if (File.Exists(fileName) == true)
                File.Delete(fileName);

            var file = File.OpenWrite(fileName);
            var writer = new StreamWriter(file);
            writer.Write(fileContent);
            writer.Flush();
            file.Close();
        }

        string readFileContent()
        {
            string fileName = getFileName();
            if (System.IO.File.Exists(fileName) == false)
                return null;

            var file = File.OpenRead(fileName);
            string fileContent = new StreamReader(file).ReadToEnd();
            file.Close();

            return fileContent;
        }

        string getFileName()
        {
            string path = this.files.GetWritableFolder();
            string fileName = Path.Combine(path, "userpreferences.txt");
            return fileName;
        }
    }
}
