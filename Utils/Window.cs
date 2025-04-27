using ClipboardUrl.Models;
using ClipboardUrl.Service.Interface;
using ClipboardUrl.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace ClipboardUrl.Utils
{
    class MessageWindow : NativeWindow
    {

        private const int WM_CLIPBOARDUPDATE = 0x031D;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateWindowEx(
            int exStyle, string className, string windowName,
            int style, int x, int y, int width, int height,
            IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hwnd);

        #region Initializer
        public MessageWindow()
        {
            CreateParams cp = new CreateParams
            {
                Caption = "ClipboardListenerWindow",
                ClassName = null,
                Style = 0x800000, // WS_OVERLAPPED
                X = 0,
                Y = 0,
                Height = 0,
                Width = 0,
                Parent = new IntPtr(-3) // HWND_MESSAGE = -3 (hidden message-only window)
            };

            this.CreateHandle(cp);
            AddClipboardFormatListener(this.Handle);
        }
        #endregion

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsText())
                {
                    Console.WriteLine("Wait until process is finished to copy again");
                    string text = Clipboard.GetText();

                    /* 
                        // 0 = none
                        // 1 = video
                        // 2 = playlist
                        // 3 = video with playlist 
                    */
                    int urlType = VideoType(text);
                    Task task = new Task(async () =>
                    {
                        try
                        {
                            switch (urlType)
                            {
                                case (int)Const.UrlType.Video:
                                    await HandleDownload(new VideoDownloaderService(), text);
                                    break;
                                case (int)Const.UrlType.Playlist:
                                    await HandleDownload(new PlaylistDownloaderService(), text);
                                    break;
                                case (int)Const.UrlType.VideoWithPlaylist:
                                    await HandleDownload(new PlaylistDownloaderService(), text);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        finally
                        {
                            Console.WriteLine("Process is finished");
                        }
                    });
                    task.Start();
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnHandleChange()
        {
            base.OnHandleChange();
            if (this.Handle != IntPtr.Zero)
            {
                AddClipboardFormatListener(this.Handle);
            }
        }

        ~MessageWindow()
        {
            if (this.Handle != IntPtr.Zero)
            {
                RemoveClipboardFormatListener(this.Handle);
                this.DestroyHandle();
            }
        }

        #region checkUrlType
        static (bool, bool) IsVideo(string text)
        {
            string url = "https://www.youtube.com/watch";
            return (text.ToLower().Contains(url), text.ToLower().Contains("list"));
        }

        static bool IsPlaylist(string text)
        {
            return text.ToLower().Contains("https://www.youtube.com/playlist");
        }

        static int VideoType(string url)
        {
            int videoType = 0;
            (bool isVideo, bool containList) = IsVideo(url);
            bool isPlaylist = IsPlaylist(url);

            if (isVideo && !containList)
            {
                videoType = (int)Const.UrlType.Video;
            }
            else if (!isVideo && containList && isPlaylist)
            {
                videoType = (int) Const.UrlType.Playlist;
            }
            else if (isVideo && containList)
            {
                videoType = (int) Const.UrlType.VideoWithPlaylist;
            }

            Display(isVideo, containList, isPlaylist);
            return videoType;
        }
        #endregion

        static void Display(bool isVideo, bool containsList, bool isPlaylist)
        {
            if (isVideo)
                Console.WriteLine("🎬 Video URL detected");
            if (isPlaylist)
                Console.WriteLine("📂 Playlist URL detected");
            if (containsList && !isPlaylist)
                Console.WriteLine("📺 Video contains playlist");
        }

        async Task HandleDownload(IDownload download,string url)
        {
            await download.Download(url);
        }
    }
}
