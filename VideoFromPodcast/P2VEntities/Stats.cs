using System;
using System.Collections.Generic;

namespace P2VEntities
{
    public class Stats
    {
        public TimeSpan Shortest { get; set; }
        public TimeSpan Longest { get; set; }
        public TimeSpan Average { get; set; }
        public long MaxChapters { get; set; }
        public long MinChapters { get; set; }
        public long AvgChapters { get; set; }

        public List<Episode> Episodes { get; set; } = new List<Episode>();
    }
}