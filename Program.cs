using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardUrl
{
    class Program
    {
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private static IntPtr _hwnd;
        private static NativeWindow _window;

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

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("🕵️‍♂️ Listening for clipboard changes (headless)...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _window = new MessageOnlyWindow();
            Application.Run(); // Message loop to receive WM_CLIPBOARDUPDATE
        }

        private class MessageOnlyWindow : NativeWindow
        {
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

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_CLIPBOARDUPDATE)
                {
                    if (Clipboard.ContainsText())
                    {
                        string text = Clipboard.GetText();
                        (bool isVideo, bool containList) = IsVideo(text);
                        bool isPlaylist = IsPlaylist(text);
                        Display(isVideo, containList, isPlaylist);
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

        static (bool, bool) IsVideo(string text)
        {
            string url = "https://www.youtube.com/watch";
            return (text.ToLower().Contains(url), text.ToLower().Contains("list"));
        }

        static bool IsPlaylist(string text)
        {
            return text.ToLower().Contains("https://www.youtube.com/playlist");
        }

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
