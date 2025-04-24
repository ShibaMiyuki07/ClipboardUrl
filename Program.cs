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

namespace ClipboardUrl
{
    class Program
    {
        #region Attributes
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private static readonly IntPtr _hwnd;
        private static NativeWindow _window;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);
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

        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.WriteLine("🕵️‍♂️ Listening for clipboard changes (headless)...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _window = new MessageOnlyWindow();
            Application.Run(); // Message loop to receive WM_CLIPBOARDUPDATE
        }

        private class MessageOnlyWindow : NativeWindow
        {
            private readonly YoutubeClient youtubeClient = new YoutubeClient();
            CancellationTokenSource cts = new CancellationTokenSource();
            public MessageOnlyWindow()
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
            static async Task ShowLoadingAsync(string message, CancellationToken token)
            {
                char[] spinner = new[] { '|', '/', '-', '\\' };
                int counter = 0;

                Console.Write(message + " ");

                while (!token.IsCancellationRequested)
                {
                    Console.Write(spinner[counter % spinner.Length]);
                    await Task.Delay(100);
                    Console.Write("\b");
                    counter++;
                }

                Console.WriteLine("✔");
            }
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_CLIPBOARDUPDATE)
                {
                    if (Clipboard.ContainsText())
                    {
                        Console.WriteLine("Wait until process is finished to copy again");
                        var loadingTask = ShowLoadingAsync("🔍 Fetching...", cts.Token);
                        RemoveClipboardFormatListener(this.Handle);
                        string text = Clipboard.GetText();

                        /* 
                            // 0 = none
                            // 1 = video
                            // 2 = playlist
                            // 3 = video with playlist 
                        */
                        int urlType = VideoType(text);
                        Task.Run(async () =>
                        {
                            switch (urlType)
                            {
                                case 1:
                                    var video = await youtubeClient.Videos.GetAsync(text);
                                    cts.Cancel();
                                    await loadingTask;
                                    break;
                                case 2:
                                    var playlist = await youtubeClient.Playlists.GetVideosAsync(text);
                                    cts.Cancel();
                                    await loadingTask;
                                    break;
                                case 3:
                                    cts.Cancel();
                                    await loadingTask;
                                    break;
                                default:
                                    break;
                            }

                            AddClipboardFormatListener(this.Handle);
                            Console.WriteLine("Process is finished");
                        });

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

            ~MessageOnlyWindow()
            {
                if (this.Handle != IntPtr.Zero)
                {
                    RemoveClipboardFormatListener(this.Handle);
                    this.DestroyHandle();
                }
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

            if(isVideo && !containList)
            {
                videoType = 1;
            }
            else if(!isVideo && containList && isPlaylist)
            {
                videoType = 2;
            }
            else if(isVideo && containList)
            {
                videoType = 3;
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
    }
}
