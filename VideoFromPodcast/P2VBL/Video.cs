using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;


// TODO: PERFORMANCE? MEMORY?


namespace P2VBL
{
    public class Video
    {
        private Image _image;

        private const string FfmpegPath = "D:\\ffmpeg\\ffmpeg.exe";
        private const string FfmpegImageParams = "-f image2pipe -framerate 1 -i pipe:.jpg -c:v libx264 -b:v 1M -vf scale=640:-1  -y";
        private const string FfmpegAudioParams = "-i IMAGEONLY -i AUDIOFILE -codec copy -shortest -y";

        public Video(Image image)
        {
            _image = image;
        }

        private MemoryStream GetAudioStream()
        {
            byte[] mp3;

            try
            {
                mp3 = new WebClient().DownloadData(_image.CurrentEpisode.Audio);
            }
            catch (Exception ex)
            {

                throw;
            }


            return new MemoryStream(mp3);
        }

        private void AddImagesToFile(Process ffmpeg)
        {
            var position = new TimeSpan();
            int imageCount = 0;
            int maxImageCount = int.MaxValue;

            while (position < _image.CurrentEpisode.Duration)
            {
                var imageFile = _image.CreateImageForTime(position);
                using (var imageStream = new MemoryStream())
                {
                    imageFile.Save(imageStream, ImageFormat.Jpeg);
                    imageStream.WriteTo(ffmpeg.StandardInput.BaseStream);
                }
                position = position.Add(TimeSpan.FromSeconds(1));
                imageCount++;
                if (imageCount > maxImageCount) break;
            }
        }

        //StartInfo = new ProcessStartInfo(FfMpegPath, $"-f image2pipe -framerate {framerate} -i pipe:.jpg -c:v libx264 -b:v 1M -vf scale=640:-1  -y D:\\output.mp4")
        private Process CreateProcess(string outputFileName, string parameters)
        {
            Process ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo(FfmpegPath, parameters+" "+outputFileName)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            ffmpeg.Start();
            return ffmpeg;
        }

        private void FinishProcess(Process ffmpeg)
        {
            ffmpeg.StandardInput.BaseStream.Flush();
            ffmpeg.StandardInput.BaseStream.Close();
            ffmpeg.WaitForExit();
        }

        public void CreateVideo()
        {
            string outputfilename = $"D:\\tmp_{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Guid.NewGuid()}";
            string noAudioFilename = outputfilename+ ".noaudio.mp4";
            string audioFilename = outputfilename + ".mp3";
            string outputfilenameFinal = outputfilename + ".mp4";

            var ffmpeg=CreateProcess(noAudioFilename, FfmpegImageParams);
            AddImagesToFile(ffmpeg);
            FinishProcess(ffmpeg);

            using (FileStream mp3File= new FileStream(audioFilename, FileMode.Create,  FileAccess.Write))
            {
                GetAudioStream().CopyTo(mp3File);
            }
            

            ffmpeg = CreateProcess(outputfilenameFinal, FfmpegAudioParams.Replace("IMAGEONLY", noAudioFilename).Replace("AUDIOFILE", audioFilename));
            //AddAudioToFile(ffmpeg);
            FinishProcess(ffmpeg);


            // Add Audio
            GetAudioStream().WriteTo(ffmpeg.StandardInput.BaseStream);
          
        }


    }
}
