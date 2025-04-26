using ClipboardUrl.Models;
using ClipboardUrl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xabe.FFmpeg;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace ClipboardUrl
{
    class Program
    {
        #region Attributes
        
        private static NativeWindow _window;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCP(uint wCodePageID);
       

        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            SetConsoleOutputCP(65001);
            SetConsoleCP(65001);
            Console.WriteLine("🕵️‍♂️ Listening for clipboard changes (headless)...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DirectoryService.CheckDirectory(Const.path);
            FFmpeg.SetExecutablesPath(Const.ffmpegPath);

            _window = new MessageWindow();
            Application.Run(); // Message loop to receive WM_CLIPBOARDUPDATE
        }

        
    }
}
