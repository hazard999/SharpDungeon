using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndTitledMessage : Window
    {
        private const int WIDTH = 120;
        private const int GAP = 2;

        private BitmapTextMultiline normal;
        private BitmapTextMultiline highlighted;

        public WndTitledMessage(Image icon, string title, string message)
            : this(new IconTitle(icon, title), message)
        {


        }

        public WndTitledMessage(Component titlebar, string message)
        {

            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var hl = new Highlighter(message);

            normal = PixelScene.CreateMultiline(hl.Text, 6);
            normal.MaxWidth = WIDTH;
            normal.Measure();
            normal.X = titlebar.Left();
            normal.Y = titlebar.Bottom() + GAP;
            Add(normal);

            if (hl.IsHighlighted)
            {
                normal.Mask = hl.Inverted();

                highlighted = PixelScene.CreateMultiline(hl.Text, 6);
                highlighted.MaxWidth = normal.MaxWidth;
                highlighted.Measure();
                highlighted.X = normal.X;
                highlighted.Y = normal.Y;
                Add(highlighted);

                highlighted.Mask = hl.Mask;
                highlighted.Hardlight(TitleColor);
            }

            Resize(WIDTH, (int)(normal.Y + normal.Height));
        }
    }

}