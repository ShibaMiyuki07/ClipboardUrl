using ClipboardUrl.Service.Interface;
using System.Threading.Tasks;
using ClipboardUrl.Models;
using YoutubeExplode.Common;

namespace ClipboardUrl.Utils
{
    public class PlaylistDownloaderService : IDownload
    {
        public async Task Download(string url)
        {
            var playlist = await Const.youtubeClient.Playlists.GetVideosAsync(url);
        }
    }
}
