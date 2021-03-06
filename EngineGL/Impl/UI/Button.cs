using System.Drawing;
using EngineGL.Structs.Drawing;

namespace EngineGL.Impl.UI
{
    public class Button : Element
    {
        public string Text { get; set; }
        public Colour4 BackColor { get; set; }
        public Font Font { get; set; }
        public int FontSize { get; set; }
        public Colour4 FontColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Button()
        {
        }

        public void Click()
        {
        }
    }
}