using System;
using System.Collections.Generic;
using System.Text;

namespace P2VEntities
{
    public class Chapter
    {
        public TimeSpan Offset { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return $"[{Offset}] {Title}";
        }
    }
}
