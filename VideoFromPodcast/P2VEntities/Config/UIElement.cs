using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace P2VEntities.Config
{
    public class UIElement
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Brush { get; set; }
        public Brush BrushValue
        {
            get
            {
                if (Brush == null) return null;
                if (int.TryParse(Brush, out int argb)) return new SolidBrush(Color.FromArgb(argb));
                return new SolidBrush(Color.FromName(Brush));
            }
        }
    }
}
