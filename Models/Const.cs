using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace ClipboardUrl.Models
{
    public static class Const
    {
        public static readonly YoutubeClient youtubeClient = new YoutubeClient();

        public static string path = @"F:\Musique";
        public enum UrlType
        {
            Video,
            Playlist,
            VideoWithPlaylist
        }
    }
}
