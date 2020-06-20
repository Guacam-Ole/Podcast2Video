using P2VEntities;

using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace P2VBL
{
    public class Video
    {
        private Image _image;

        private TimeSpan _start;
        private TimeSpan _finish;
        private Chapter _chapter;

        private P2VEntities.Config.FFMpeg _ffmpegConfig = Config.Configuration.FFMpeg;


        private Video(Image image, int? chapterId, TimeSpan? start, TimeSpan? finish)
        {
            _image = image;
            _start = start ?? new TimeSpan();
            _finish = finish ?? _image.CurrentEpisode.Duration;
            if (chapterId != null)
            {
                _chapter = _image.CurrentEpisode.Chapters.ElementAt(chapterId.Value);
                var nextChapterId = chapterId.Value + 1;
                if (nextChapterId > _image.CurrentEpisode.Chapters.Count)
                {
                    _finish = _image.CurrentEpisode.Duration;
                }
                else
                {
                    _finish = _image.CurrentEpisode.Chapters.ElementAt(nextChapterId).Offset;
                }
                _start = _chapter.Offset;
            }
        }

        public Video(Image image) : this(image, null, null, null)
        {
        }

        public Video(Image image, int chapter) : this(image, chapter, null, null)
        {
        }

        public Video(Image image, TimeSpan start, TimeSpan finish) : this(image, null, start, finish)
        {
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

     
        private string Dos(string filename)
        {
            return "\"" + filename + "\"";
        }

        private void AddAudio(string baseFilename)
        {
            string audioFilename = baseFilename + ".mp3";
            string audioFilenameCrop = baseFilename + ".crop.mp3";
            string outputfilenameFinal = baseFilename + ".mp4";
            string slideshowFilename = baseFilename + ".noaudio.mp4";

            using (FileStream mp3File = new FileStream(audioFilename, FileMode.Create, FileAccess.Write))
            {
                GetAudioStream().CopyTo(mp3File);
            }

            var ffmpeg = CreateProcess(audioFilenameCrop, _ffmpegConfig.CropAudio.Replace("AUDIOFILE", Dos(audioFilename)).Replace("START",_start.ToString()).Replace("FINISH",_finish.ToString()));
            FinishProcess(ffmpeg);

            ffmpeg = CreateProcess(outputfilenameFinal, _ffmpegConfig.AddAudio.Replace("IMAGEONLY", Dos( slideshowFilename)).Replace("AUDIOFILE", Dos( audioFilenameCrop)));
            FinishProcess(ffmpeg);
            CleanupFiles(audioFilenameCrop, slideshowFilename, audioFilename);
        }

        private void AddImagesToFile(Process ffmpeg)
        {
            var position = _start;
            int imageCount = 0;
            int maxImageCount = int.MaxValue;

            while (position < _finish)
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

        private Process CreateProcess(string outputFileName, string parameters)
        {
            Process ffmpeg = new Process
            {
                StartInfo = new ProcessStartInfo(_ffmpegConfig.Path, parameters + " \"" + outputFileName+"\"")
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

        private string GetFilenameFromEpisode()
        {
            string filename = $"{_image.Podcast.Title} - {_image.CurrentEpisode.Title}";
            if (_chapter != null) filename += " " + _chapter.Title;
            return Path.Combine(_ffmpegConfig.TmpDirectory, filename.ToFilename());
        }

        public void CreateVideo()
        {
            string outputfilename = GetFilenameFromEpisode();
            string noAudioFilename = outputfilename + ".noaudio.mp4";

            var ffmpeg = CreateProcess(noAudioFilename, _ffmpegConfig.Slideshow);
            AddImagesToFile(ffmpeg);
            FinishProcess(ffmpeg);
            AddAudio(outputfilename);

        }

        private void CleanupFiles(params string[] filenames)
        {
            foreach (var filename in filenames)
            {
                System.IO.File.Delete(filename);
            }
        }
    }
}