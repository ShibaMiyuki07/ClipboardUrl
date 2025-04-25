using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardUrl.Service.Interface
{
    public interface IDownload
    {
        Task Download(string url);
    }
}
