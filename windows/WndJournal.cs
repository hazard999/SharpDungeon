using Java.Util;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndJournal : Window
    {
        private const int WIDTH = 112;
        private const int HEIGHT = 160;

        private const int ItemHeight = 18;

        private const string TxtTitle = "Journal";

        private readonly BitmapText txtTitle;
        private readonly ScrollPane list;

        public WndJournal()
        {
            Resize(WIDTH, HEIGHT);

            txtTitle = PixelScene.CreateText(TxtTitle, 9);
            txtTitle.Hardlight(TitleColor);
            txtTitle.Measure();
            txtTitle.X = PixelScene.Align(PixelScene.uiCamera, (WIDTH - txtTitle.Width) / 2);
            Add(txtTitle);

            var content = new Component();

            Collections.Sort(Journal.Records);

            float pos = 0;
            foreach (var rec in Journal.Records)
            {
                var item = new ListItem(rec.feature, rec.depth);
                item.SetRect(0, pos, WIDTH, ItemHeight);
                content.Add(item);

                pos += item.Height;
            }

            content.SetSize(WIDTH, pos);

            list = new ScrollPane(content);
            Add(list);

            list.SetRect(0, txtTitle.Height, WIDTH, HEIGHT - txtTitle.Height);
        }

        private class ListItem : Component
        {
            private BitmapText _feature;
            private BitmapText _depth;

            private Image _icon;

            public ListItem(Journal.Feature f, int d)
            {
                _feature.Text(f.desc);
                _feature.Measure();

                _depth.Text(d.ToString());
                _depth.Measure();

                if (d != Dungeon.Depth)
                    return;

                _feature.Hardlight(TitleColor);
                _depth.Hardlight(TitleColor);
            }

            protected override void CreateChildren()
            {
                _feature = PixelScene.CreateText(9);
                Add(_feature);

                _depth = new BitmapText(PixelScene.font1x);
                Add(_depth);

                _icon = Icons.DEPTH.Get();
                Add(_icon);
            }

            protected override void Layout()
            {
                _icon.X = Width - _icon.Width;

                _depth.X = _icon.Y - 1 - _depth.Width;

                _depth.Y = PixelScene.Align(Y + (Height - _depth.Height) / 2);

                _icon.Y = _depth.Y - 1;

                _feature.Y = PixelScene.Align(_depth.Y + _depth.BaseLine() - _feature.BaseLine());
            }
        }
    }
}