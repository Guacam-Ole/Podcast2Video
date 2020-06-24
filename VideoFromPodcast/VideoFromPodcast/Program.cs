using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using P2VBL;

using P2VEntities;

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

        static void Main(string[] args)
        {
            StartupParameters startupParameters;
            try
            {
                startupParameters = new StartupParameters(args);
            }
            catch 
            {
                Console.WriteLine("usage:");
                Console.WriteLine("p2v <url> <options>");
                Console.WriteLine("Example:");
                Console.WriteLine("p2v https://www.blathering.de/feed/mp3");
                Console.WriteLine("Go to https://github.com/OleAlbers/Podcast2Video/wiki for a complete list of options");
                return;
            }

            var config = new Config();
            config.OverrideConfig(args);
            if (startupParameters.Verbose) Console.WriteLine($"Config: {JsonConvert.SerializeObject(Config.Configuration)}");

            Rss rss;

            if (startupParameters.Rss != null)
            {
                rss = new Rss(new Uri(startupParameters.Rss));
            }
            else
            {
                rss = new Rss(startupParameters.LocalXml);
            }

            var output = new Image(rss.Podcast, startupParameters.Episode);

            Video video;
            if (startupParameters.Chapter != null)
            {
                var matchingChapter = output.CurrentEpisode.GetChapterAt(startupParameters.Chapter.Value);
                if (matchingChapter == null) throw new Exception($"No chapter at {startupParameters.Chapter}");
                video = new Video(output, output.CurrentEpisode.Chapters.IndexOf(matchingChapter));
            }
            else if (startupParameters.Start == null && startupParameters.Finish == null)
            {
                video = new Video(output);
            }
            else
            {
                if (startupParameters.Start == null) startupParameters.Start = new TimeSpan();
                if (startupParameters.Finish == null) startupParameters.Finish = output.CurrentEpisode.Duration;
                video = new Video(output, startupParameters.Start.Value, startupParameters.Finish.Value);
            }
            video.CreateVideo(startupParameters.StoreImages, startupParameters.OutputFilename, startupParameters.LocalMp3);

        }
    }
}
