using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardUrl.Utils
{
    class ProgressTask
    {
        public IProgress<bool> progress = new Progress<bool>(percent =>
        {
            // This callback will be invoked with progress updates
            Console.WriteLine($"Completed: {percent}");
        });
    }
}
