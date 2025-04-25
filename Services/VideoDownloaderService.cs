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
using Id3;

namespace ClipboardUrl.Services
{
    public class VideoDownloaderService : IDownload
    {
        CancellationToken token = new CancellationToken();
        public async Task Download(string url)
        {
            var video = await Const.youtubeClient.Videos.GetAsync(url);
            var streamInfo = await Const.youtubeClient.Videos.Streams.GetManifestAsync(url);
            var audio = streamInfo.GetAudioOnlyStreams().GetWithHighestBitrate();


            DownloadFile downloadFile = DownloadFile.initializeDownloadFile(video);


            if(!Directory.Exists(Const.path))
                Directory.CreateDirectory(Const.path);
            string fullpath = Path.Combine(Const.path, $@"{downloadFile.Title}.mp3");
            if(!File.Exists(fullpath))
            {
                using (var audioStream = File.Create(fullpath))
                {
                    await Const.youtubeClient.Videos.Streams.CopyToAsync(audio, audioStream, cancellationToken: token);
                }
                using (var mp3 = new Mp3(fullpath,Mp3Permissions.ReadWrite))
                {
                    Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
                    if(tag == null)
                    {
                        tag = new Id3Tag();
                    }
                    tag.Title = downloadFile.Title;
                    tag.Publisher = downloadFile.Author;
                    mp3.WriteTag(tag, WriteConflictAction.Replace);
                }
            }
            
        }
        

    }
}
