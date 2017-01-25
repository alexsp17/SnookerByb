using System;
using System.IO;
using Xamarin.Forms;
using Awpbs.Mobile;
using Mono.Data.Sqlite;

namespace Awpbs.Mobile.iOS
{
    public class Files_iOS : IFiles
    {
        public Files_iOS()
        {
        }

        public string GetWritableFolder()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return path;
        }

        public bool DoesFileExist(string fileName)
        {
            return System.IO.File.Exists(fileName);
        }

        public void DeleteFile(string fileName)
        {
            System.IO.File.Delete(fileName);
        }

        public void CreateFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                return;
            System.IO.File.Create(fileName);
        }
    }
}
