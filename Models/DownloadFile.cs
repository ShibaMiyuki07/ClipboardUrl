using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace ClipboardUrl.Models
{
    public class DownloadFile
    {
        public string Author { get; set; } = string.Empty;

        public string Title {  get; set; } = string.Empty;

        public static DownloadFile initializeDownloadFile(Video video)
        {
            DownloadFile downloadFile = new DownloadFile();
            downloadFile.Title = Regex.Replace(video.Title, @"[^a-zA-Z0-9\s]", "");
            downloadFile.Author = video.Author.ChannelTitle;
            return downloadFile;
        }
    }
}
