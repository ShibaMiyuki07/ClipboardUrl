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
using System.Net.Http;

namespace ClipboardUrl.Services
{
    public class VideoDownloaderService : IDownload
    {
        public async Task Download(string url)
        {

            (var video, var audio) = await VideoInfo.GetInfo(url);


            DownloadFile downloadFile = DownloadFile.InitializeDownloadFile(video,Const.path);
            
            string audioFullPath = Path.Combine(Const.path, $@"{downloadFile.FullTitle}.{audio.Container.Name}");
            await GetCover(downloadFile, Const.path);
            await DownloadAudio(audioFullPath, audio);

            
            await AudioConverter.ConvertAudioToMp3(audioFullPath, downloadFile);
        }
        
        public static async Task GetCover(DownloadFile download,string path)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync($"https://img.youtube.com/vi/{download.Id}/maxresdefault.jpg");
                    string imagePath = download.CoverPath;
                    File.WriteAllBytes(imagePath, imageBytes);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task DownloadAudio(string audioFullPath,IStreamInfo audio)
        {
            try
            {
                await Const.youtubeClient.Videos.Streams.DownloadAsync(audio, audioFullPath);
            }
            catch(Exception e)
            {
                Console.Write(audioFullPath);
                Console.WriteLine(e.Message);
            }
        }
    }
}
