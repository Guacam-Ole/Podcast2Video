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

            if (startupParameters.JustCount)
            {
                int episodeCount = 0;
                Episode oldesEpisode = new Episode { Published = DateTime.MaxValue };
                Episode newestEpisode = new Episode { Published = DateTime.MinValue };
                Episode shortestEpisode = new Episode { Duration = TimeSpan.MaxValue };
                Episode longestEpisode = new Episode();
                TimeSpan totalDuration = new TimeSpan();
                foreach (var episode in rss.Podcast.Episodes)
                {
                    episodeCount++;
                    if (episode.Duration < shortestEpisode.Duration && episode.Duration.TotalSeconds>0) shortestEpisode = episode;
                    if (episode.Duration > longestEpisode.Duration) longestEpisode = episode;
                    if (episode.Published < oldesEpisode.Published) oldesEpisode = episode;
                    if (episode.Published > newestEpisode.Published) newestEpisode = episode;
                    totalDuration += episode.Duration;
                }
                Console.WriteLine($"Statistiics for {rss.Podcast.Title} ({startupParameters.Rss}):");
                Console.WriteLine($"Shortest Episode: \t {shortestEpisode} ({shortestEpisode.Duration})");
                Console.WriteLine($"Longest Episode:  \t {longestEpisode} ({longestEpisode.Duration})");
                Console.WriteLine($"First Episode:    \t {oldesEpisode}");
                Console.WriteLine($"Last Episode:     \t {newestEpisode}");
                Console.WriteLine($"Total Duration:   \t {totalDuration} (in {episodeCount} episodes)");

                Console.ReadLine();
                return;
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
