using ClipboardUrl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardUrl.Utils
{
    class DirectoryService
    {
        public static void CheckDirectory()
        {
            if (!Directory.Exists(Const.path))
                Directory.CreateDirectory(Const.path);
        }
    }
}
