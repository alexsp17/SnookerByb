using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Awpbs.Mobile
{
    public class KeyChain_UnsecuredFile : IKeyChain
    {
        IFiles files;
        Dictionary<string, string> dictionary;

        public KeyChain_UnsecuredFile(IFiles files)
        {
            this.files = files;
        }

        public override bool Add(string key, string value)
        {
            try
            {
                loadDictionary();
                if (dictionary.ContainsKey(key))
                    dictionary.Remove(key);

                dictionary.Add(key, value);

                saveDictionary();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void Delete(string key)
        {
            try
            {
                loadDictionary();
                if (dictionary.ContainsKey(key))
                    dictionary.Remove(key);

                saveDictionary();
            }
            catch (Exception)
            {
            }
        }

        public override string Get(string key)
        {
            try
            {
                if (dictionary == null)
                    this.loadDictionary();

                if (dictionary.ContainsKey(key))
                    return dictionary[key];

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        void loadDictionary()
        {
            try
            {
                string fileContent = this.readFileContent();
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);
            }
            catch (Exception)
            {
                dictionary = new Dictionary<string, string>();
            }
        }

        void saveDictionary()
        {
            string fileContent = "";
            if (dictionary != null)
                fileContent = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);

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
            string fileName = Path.Combine(path, "keychain.txt");
            return fileName;
        }
    }
}
