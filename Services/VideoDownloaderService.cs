using ClipboardUrl.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using YoutubeExplode;
using ClipboardUrl.Models;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using ClipboardUrl.Utils;
using System.IO;
using System.Threading;
using Xabe.FFmpeg;

namespace ClipboardUrl.Services
{
    public class VideoDownloaderService : IDownload
    {
        public async Task Download(string url)
        {

            (var video, var audio) = await VideoInfo.GetInfo(url);


            DownloadFile downloadFile = DownloadFile.InitializeDownloadFile(video);

            string audioFullPath = Path.Combine(Const.path, $@"{downloadFile.Title}.{audio.Container.Name}");

            await DownloadAudio(audioFullPath, audio);

            Console.WriteLine("Processing mp3 conversion");
            await AudioConverter.ConvertAudioToMp3(audioFullPath, downloadFile);
            Console.WriteLine("Mp3 conversion finished");
            FileService.DeleteFile(audioFullPath);
        }
        
        private async Task DownloadAudio(string audioFullPath,IStreamInfo audio)
        {
            await Const.youtubeClient.Videos.Streams.DownloadAsync(audio, audioFullPath);
        }

        
    }
}
