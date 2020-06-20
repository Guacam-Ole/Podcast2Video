using System.Drawing;

namespace P2VEntities.Config
{
    public class TextElement : UIElement
    {
        public FontElement Font { get; set; }

        public Font SystemFont
        {
            get
            {
                return new Font(Font.Name, Font.Size);
            }
        }
    }
}