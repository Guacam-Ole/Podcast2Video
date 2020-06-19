using P2VBL;

using System;
using System.Drawing.Imaging;

namespace VideoFromPodcast
{
    class Program
    {
        static void Main(string[] args)
        {
            var rss=new Rss("https://www.blathering.de/feed/mp3/");
            var output=new Image(rss.Podcast);
            //output.CreateBackgroundImage();
            //var position = new TimeSpan();

            //while (position<output.CurrentEpisode.Duration) 
            //{
            //    var img = output.CreateImageForTime(position);
            //    img.Save($"D:\\tmpimg\\{position.TotalSeconds}.jpg", ImageFormat.Jpeg);
            //    position=position.Add(TimeSpan.FromSeconds(10));
            //}
            var video=new Video(output);
            video.CreateVideo();

        }
    }
}
