using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace ClipboardUrl.Models
{
    public static class Const
    {
        public static readonly YoutubeClient youtubeClient = new YoutubeClient();

        public static string path = @"D:\Musique";

        public static readonly string ffmpegPath = Path.Combine(AppContext.BaseDirectory,"ffmpeg/bin");
        public enum UrlType
        {
            Video,
            Playlist,
            VideoWithPlaylist
        }
    }
}
