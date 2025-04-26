using ClipboardUrl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace ClipboardUrl.Utils
{
    class AudioConverter
    {
        public static async Task ConvertAudioToMp3(string audioFullPath,DownloadFile downloadFile)
        {
            Console.WriteLine("Processing mp3 conversion");
            string parameter = $"-i \"{audioFullPath}\" -movflags use_metadata_tags -map_metadata 0 -metadata title=\"{downloadFile.Title}\" -metadata artist=\"{String.Join(",",downloadFile.Author)}\" \"{audioFullPath.Replace("webm", "mp3")}\"";
            try
            {
                IConversion conversion = FFmpeg.Conversions.New();
                conversion.OnProgress += (sender, args) =>
                {
                    var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                    Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
                };
                await conversion.Start(parameter);
                Console.WriteLine("Mp3 conversion finished");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
