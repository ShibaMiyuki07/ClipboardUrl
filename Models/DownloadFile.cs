using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos;

namespace ClipboardUrl.Models
{
    public class DownloadFile
    {
        public string Id { get; set; } = string.Empty;
        public string[] Author { get; set; } = new string[] { };

        public string FilePath { get; set; } = string.Empty;
        public string FullTitle { get; set; } = string.Empty;

        public string Title {  get; set; } = string.Empty;

        public string CoverPath { get; set; } = string.Empty;

        public string Album { get; set; } = string.Empty;

        private static readonly string[] ArtistsSeparators = new string[] { "&", "feat.", "feat", "ft.", " ft ", "Feat.", " x ", " X " };

        public static DownloadFile InitializeDownloadFile(Video video,string path)
        {
            DownloadFile downloadFile = new DownloadFile
            {
                Id = video.Id,
                FullTitle = string.Join("", video.Title.Split(Path.GetInvalidFileNameChars())),
                FilePath = path,
                CoverPath = Path.Combine(path, video.Id + "_cover.jpg")
            };
            (downloadFile.Author,downloadFile.Title) = GetAuthorAndTitleFromVideo(video);
            return downloadFile;
        }

        #region Private methods

        private static (string[],string) GetAuthorAndTitleFromVideo(Video video)
        {
            string[] authors = new string[] { };
            string title = string.Empty;
            var index = video.Title.LastIndexOf('-');
            if (index > 0)
            {
                title = video.Title.Substring(index + 1).Trim(' ', '-');

                if (string.IsNullOrWhiteSpace(title))
                {
                    index = video.Title.IndexOf('-');
                    if (index > 0)
                    {
                        title = video.Title.Substring(index + 1).Trim(' ', '-');
                    }
                }

                authors = video.Title.Substring(0, index - 1).Trim().Split(ArtistsSeparators, StringSplitOptions.RemoveEmptyEntries);

            }
            return (authors,title);
        }

        #endregion
    }
}
