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
            try
            {
                //Get all the playlist data
                var playlist = await Const.youtubeClient.Playlists.GetAsync(url);
                var playlistVideos = await Const.youtubeClient.Playlists.GetVideosAsync(url);

                var directory = Path.Combine(Const.path, string.Join("", playlist.Title.Split(Path.GetInvalidFileNameChars())));
                DirectoryService.CheckDirectory(directory);

                await LaunchTheDownload(playlistVideos, directory);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        #region Private Method
        private async Task LaunchTheDownload(IReadOnlyList<PlaylistVideo> playlistVideos,string directory)
        {
            try
            {
                List<Task> tasksDownloadAndConvert = new List<Task>();
                foreach (var list in playlistVideos)
                {
                    try
                    {
                        tasksDownloadAndConvert.Add(DownloadAndConvert(directory, list));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
                await Task.WhenAll(tasksDownloadAndConvert);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        private async Task DownloadAndConvert(string directory,PlaylistVideo list)
        {
            try
            {
                (var audioFullPath, var audio, var downloadFile) = await PrepareDownloadAndConvert(list, directory);
                await VideoDownloaderService.GetCover(downloadFile, directory);
                await VideoDownloaderService.DownloadAudio(audioFullPath, audio);
                await AudioConverter.ConvertAudioToMp3(audioFullPath, downloadFile);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
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
