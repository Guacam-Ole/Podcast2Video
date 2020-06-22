using Microsoft.Extensions.Hosting;

using P2VBL;

using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Threading;

namespace VideoFromPodcast
{
    class Program
    {
        private static bool _paintImages = false;
        private static string _outputFilename=null;

        private static void GetStartupParamters(string[] args, out string episode, out TimeSpan? chapter, out TimeSpan? start, out TimeSpan? finish)
        {
            episode = null;
            chapter = null;
            start = null;
            finish = null;

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (args[i] == "-episode") episode = args[i + 1];
                    if (args[i] == "-chapter") chapter = args[i + 1].ToTimeSpan();
                    if (args[i] == "-start") start = args[i + 1].ToTimeSpan();
                    if (args[i] == "-finish") start = args[i + 1].ToTimeSpan();
                    if (args[i] == "-images") _paintImages = true;
                    if (args[i] == "-output") _outputFilename = args[i + 1];
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error on parameter '{args[i]}'", ex);
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage:");
                Console.WriteLine("p2v <url> <options>");
                Console.WriteLine("Example:");
                Console.WriteLine("p2v https://www.blathering.de/feed/mp3");
                Console.WriteLine("Go to https://github.com/OleAlbers/Podcast2Video/wiki for a complete list of options");
                return;
            }

            GetStartupParamters(args, out string episode, out TimeSpan? chapter, out TimeSpan? start, out TimeSpan? finish);
            var config = new Config();
            config.OverrideConfig(args);

            var rss = new Rss(args[0]);
            var output = new Image(rss.Podcast, episode);
            
            Video video;
            if (chapter!=null)
            {
                var matchingChapter = output.CurrentEpisode.GetChapterAt(chapter.Value);
                if (matchingChapter == null) throw new Exception($"No chapter at {chapter}");
                video = new Video(output, output.CurrentEpisode.Chapters.IndexOf(matchingChapter));
            } else if (start==null && finish==null)
            {
                video = new Video(output);
            } else
            {
                if (start == null) start = new TimeSpan();
                if (finish == null) finish = output.CurrentEpisode.Duration;
                video = new Video(output, start.Value, finish.Value);
            }
            video.CreateVideo(_paintImages, _outputFilename);

        }
    }
}
