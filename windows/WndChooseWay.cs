using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndChooseWay : Window
    {
        private const string TxtMessage = "Which way will you follow?";
        private const string TxtCancel = "I'll decide later";

        private const int WIDTH = 120;
        private const int BtnHeight = 18;
        private const float Gap = 2;

        public WndChooseWay(TomeOfMastery tome, HeroSubClass way1, HeroSubClass way2)
        {
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(tome.Image, null));
            titlebar.Label(tome.Name);
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var hl = new Highlighter(way1.Desc + "\\Negative\\Negative" + way2.Desc + "\\Negative\\Negative" + TxtMessage);

            var normal = PixelScene.CreateMultiline(hl.Text, 6);
            normal.MaxWidth = WIDTH;
            normal.Measure();
            normal.X = titlebar.Left();
            normal.Y = titlebar.Bottom() + Gap;
            Add(normal);

            if (hl.IsHighlighted)
            {
                normal.Mask = hl.Inverted();

                var highlighted = PixelScene.CreateMultiline(hl.Text, 6);
                highlighted.MaxWidth = normal.MaxWidth;
                highlighted.Measure();
                highlighted.X = normal.X;
                highlighted.Y = normal.Y;
                Add(highlighted);

                highlighted.Mask = hl.Mask;
                highlighted.Hardlight(TitleColor);
            }

            var btnWay1 = new RedButton(Utils.Capitalize(way1.Title));
            btnWay1.ClickAction = button =>
            {
                Hide();
                tome.Choose(way1);
            };
            btnWay1.SetRect(0, normal.Y + normal.Height + Gap, (WIDTH - Gap) / 2, BtnHeight);
            Add(btnWay1);

            var btnWay2 = new RedButton(Utils.Capitalize(way2.Title));
            btnWay2.ClickAction = button =>
            {
                Hide();
                tome.Choose(way2);
            };
            btnWay2.SetRect(btnWay1.Right() + Gap, btnWay1.Top(), btnWay1.Width, BtnHeight);
            Add(btnWay2);

            var btnCancel = new RedButton(TxtCancel);
            btnCancel.ClickAction = button => Hide();
            btnCancel.SetRect(0, btnWay2.Bottom() + Gap, WIDTH, BtnHeight);
            Add(btnCancel);

            Resize(WIDTH, (int)btnCancel.Bottom());
        }
    }
}