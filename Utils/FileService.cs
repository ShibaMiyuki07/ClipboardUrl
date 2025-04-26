using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardUrl.Utils
{
    class FileService
    {
        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
