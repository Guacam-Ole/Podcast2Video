using System;
using System.Collections.Generic;
using System.Text;

namespace P2VEntities.Config
{
    public class Episode
    {
        public TextElement Title { get; set; }
        public TextElement Desription { get; set; }
        public TextElement CurrentChapter { get; set; }
        public TextElement OtherChapter { get; set; }
        public BlockElement Timeline { get; set; }
        public BlockElement TimelineActive { get; set; }
        public BlockElement TImelineChapter { get; set; }
        public BlockElement TimelineChapterActive { get; set; }
        public BlockElement Time { get; set; }

    }
}
