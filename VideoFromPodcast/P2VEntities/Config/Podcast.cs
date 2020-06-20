using System;
using System.Collections.Generic;
using System.Text;

namespace P2VEntities.Config
{
    public class Podcast
    {
        public BlockElement Background { get; set; }
        public BlockElement Icon { get; set; }
        public TextElement Title { get; set; }
        public TextElement Description { get; set; }
        public Episode Episode { get; set; }
    }
}
