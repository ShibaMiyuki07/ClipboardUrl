using ClipboardUrl.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            try
            {
                string parameter = InitializeParameter(audioFullPath, downloadFile);


                IConversion conversion = FFmpeg.Conversions.New();
                await conversion.Start(parameter);
                FileService.DeleteFile(downloadFile.CoverPath);
                FileService.DeleteFile(audioFullPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string InitializeParameter(string audioFullPath,DownloadFile downloadFile)
        {
            string coverParameter = string.Empty;
            if (File.Exists(downloadFile.CoverPath))
            {
                coverParameter = $"-i \"{downloadFile.CoverPath}\" -map 0:a -map 1:v -c:v mjpeg -id3v2_version 3";
            }
            string extensions = audioFullPath.Split('.')[audioFullPath.Split('.').Length - 1];
            string parameter = $"-i \"{audioFullPath}\" {coverParameter}  -movflags use_metadata_tags -map_metadata 0 -metadata title=\"{(!string.IsNullOrEmpty(downloadFile.Title) ? downloadFile.Title : "")}\" -metadata artist=\"{(downloadFile.Author.Length > 0 ? string.Join(",", downloadFile.Author) : "")}\" \"{audioFullPath.Replace(extensions, "mp3")}\"";
            return parameter;
        }
    }
}
