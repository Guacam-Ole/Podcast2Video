using System;
using System.Collections.Generic;

namespace P2VEntities
{
    public class Episode
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Audio { get; set; }
        public string Unique { get; set; }
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }
        public List<Chapter> Chapters { get; set; }
        public DateTime Published { get; set; }

        public override string ToString()
        {
            return $"[{Published}]\t{Title}\t({Link})";
        }

    }
}
