using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
    public interface IFiles
    {
        string GetWritableFolder();
        bool DoesFileExist(string fileName);
        void CreateFile(string fileName);
        void DeleteFile(string fileName);
    }
}
