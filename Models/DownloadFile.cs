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
        public string[] Author { get; set; }

        public string Title {  get; set; } = string.Empty;

        private static readonly string[] ArtistsSeparators = new string[] { "&", "feat.", "feat", "ft.", " ft ", "Feat.", " x ", " X " };

        public static DownloadFile InitializeDownloadFile(Video video)
        {
            DownloadFile downloadFile = new DownloadFile();
            var index = video.Title.LastIndexOf('-');

            if (index > 0)
            {
                downloadFile.Title = video.Title.Substring(index + 1).Trim(' ', '-');

                if (string.IsNullOrWhiteSpace(downloadFile.Title))
                {
                    index = video.Title.IndexOf('-');

                    if (index > 0)
                    {
                        downloadFile.Title = video.Title.Substring(index + 1).Trim(' ', '-');
                    }
                }

                downloadFile.Author = video.Title.Substring(0, index - 1).Trim().Split(ArtistsSeparators, StringSplitOptions.RemoveEmptyEntries);

            }
            return downloadFile;
        }


    }
}
