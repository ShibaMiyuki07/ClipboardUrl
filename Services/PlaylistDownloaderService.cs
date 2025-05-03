using ClipboardUrl.Service.Interface;
using System.Threading.Tasks;
using ClipboardUrl.Models;
using YoutubeExplode.Common;
using System.IO;
using System.Collections.Generic;
using ClipboardUrl.Services;
using YoutubeExplode.Videos.Streams;
using System;
using YoutubeExplode.Playlists;
using System.Threading;
using System.Linq;

namespace ClipboardUrl.Utils
{
    public class PlaylistDownloaderService : IDownload
    {
        public async Task Download(string url)
        {
            //Get all the playlist data
            var playlist = await Const.youtubeClient.Playlists.GetAsync(url);
            var playlistVideos = await Const.youtubeClient.Playlists.GetVideosAsync(url);

            var directory = Path.Combine(Const.path, string.Join("", playlist.Title.Split(Path.GetInvalidFileNameChars())));
            DirectoryService.CheckDirectory(directory);

            await LaunchTheDownload(playlistVideos, directory);
        }

        #region Private Method
        private async Task LaunchTheDownload(IReadOnlyList<PlaylistVideo> playlistVideos,string directory)
        {
            List<Task> tasksDownloadAndConvert = new List<Task>();
            foreach (var list in playlistVideos)
            {
                    tasksDownloadAndConvert.Add(DownloadAndConvert(directory, list));
            }
            await Task.WhenAll(tasksDownloadAndConvert);
        }

        private async Task DownloadAndConvert(string directory,PlaylistVideo list)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(100, 500)));
            (var audioFullPath, var audio, var downloadFile) = await PrepareDownloadAndConvert(list, directory);
            await VideoDownloaderService.GetCover(downloadFile, directory);
            await VideoDownloaderService.DownloadAudio(audioFullPath, audio);
            await AudioConverter.ConvertAudioToMp3(audioFullPath, downloadFile);
        }

        private async Task<(string, IStreamInfo, DownloadFile)> PrepareDownloadAndConvert(PlaylistVideo list,string directory)
        {
            (var video, var audio) = await VideoInfo.GetInfo(list.Url);
            
            DownloadFile downloadFile = DownloadFile.InitializeDownloadFile(video,directory);
            string audioFullPath = Path.Combine(directory, $@"{downloadFile.FullTitle}.{audio.Container.Name}");

            return (audioFullPath,audio,downloadFile);
        }

        #endregion
    }
}
