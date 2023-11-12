using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

using Newtonsoft.Json;

using P2VBL;

namespace P2VEntities
{
    public class StartupParameters
    {
        public string LocalXml { get; set; }
        public string Rss { get; set; }
        public string LocalMp3 { get; set; }
        public string Episode { get; set; }
        public int? EpisodeId { get; set; }
        public TimeSpan? Chapter { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? Finish { get; set; }
        public bool StoreImages { get; set; }
        public string OutputFilename { get; set; }
        public bool Verbose { get; set; }
        public bool JustCount { get; set; }
        public string Ignore { get; set; }

        public StartupParameters(string[] args)
        {
            if (args.Length ==0) throw new Exception("No RSS - feed");
            if (!args[0].StartsWith("-")) Rss = args[0];

            string EpisodeIdVal = args.GetNextStringFor("-episodeid");
            if (EpisodeIdVal != null)
            {
                EpisodeId=int.Parse(EpisodeIdVal);
            }
            Episode = args.GetNextStringFor("-episode");
            Chapter = args.GetNextTimespanFor("-chapter");
            Start = args.GetNextTimespanFor("-start");
            Finish = args.GetNextTimespanFor("-finish");
            OutputFilename = args.GetNextStringFor("-output");
            LocalXml = args.GetNextStringFor("-xml");
            LocalMp3 = args.GetNextStringFor("-mp3");
            StoreImages = args.Contains("-images");
            Verbose = args.Contains("-verbose");
            JustCount = args.Contains("-count");
            Ignore = args.GetNextStringFor("-ignore");

            if (Verbose) Console.WriteLine($"Startup: {JsonConvert.SerializeObject(this)}");
        }
    }
}
