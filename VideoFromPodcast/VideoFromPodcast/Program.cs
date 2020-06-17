using P2VBL;

using System;

namespace VideoFromPodcast
{
    class Program
    {
        static void Main(string[] args)
        {
            var rss=new Rss("https://www.blathering.de/feed/mp3/");
            var output=new Output(rss.Podcast);
            output.CreateImage();
        }
    }
}
