using System;
using System.Collections.Generic;
using System.Text;

namespace P2VEntities.Config
{
    public class FFMpeg
    {
        public string Path { get; set; }
        public string TmpDirectory { get; set; }
        public string Slideshow { get; set; }
        public string AddAudio {get;set;}
        public string CropAudio { get; set; }
    }
}
