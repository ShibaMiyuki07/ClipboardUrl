using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            downloadFile.Title = string.Join("", video.Title.Split(Path.GetInvalidPathChars()));
            downloadFile.Author = video.Author.ChannelTitle;
            string[] titleSplit = video.Title.Split('-');
            if (titleSplit.Length > 0) { 
                downloadFile.Author = titleSplit[0];
                downloadFile.Title = string.Join("", titleSplit[1].Split(Path.GetInvalidPathChars()));
            }
            return downloadFile;
        }
    }
}
