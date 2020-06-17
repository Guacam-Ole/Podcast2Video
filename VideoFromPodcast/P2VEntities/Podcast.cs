using System;
using System.Collections.Generic;
using System.Text;

namespace P2VEntities
{
    public class Podcast
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public List<Episode> Episodes { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
