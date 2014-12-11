using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndMessage : Window
    {
        private const int WIDTH = 120;
        private const int MARGIN = 4;

        public WndMessage(string text)
        {
            var info = PixelScene.CreateMultiline(text, 6);
            info.MaxWidth = WIDTH - MARGIN * 2;
            info.Measure();
            info.X = info.Y = MARGIN;
            Add(info);

            Resize((int)info.Width + MARGIN * 2, (int)info.Height + MARGIN * 2);
        }
    }

}