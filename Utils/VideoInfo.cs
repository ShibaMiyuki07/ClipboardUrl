using ClipboardUrl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;

namespace ClipboardUrl.Utils
{
    class VideoInfo
    {
        public static async Task<(Video, IStreamInfo)> GetInfo(string url)
        {
            var video = await Const.youtubeClient.Videos.GetAsync(url);
            var streamInfo = await Const.youtubeClient.Videos.Streams.GetManifestAsync(url);
            var audio = streamInfo.GetAudioOnlyStreams().GetWithHighestBitrate();

            return (video, audio);
        }
    }
}
