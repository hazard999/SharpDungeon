using System;
using pdsharp.noosa;
using sharpdungeon.effects;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndBadge : Window
    {
        private const int WIDTH = 120;
        private const int MARGIN = 4;

        public WndBadge(Badge badge)
        {
            var icon = BadgeBanner.Image(badge.Image);
            icon.Scale.Set(2);
            Add(icon);

            var info = PixelScene.CreateMultiline(badge.Description, 8);
            info.MaxWidth = WIDTH - MARGIN * 2;
            info.Measure();

            var w = Math.Max(icon.Width, info.Width) + MARGIN * 2;

            icon.X = (w - icon.Width) / 2;
            icon.Y = MARGIN;

            var pos = icon.Y + icon.Height + MARGIN;
            foreach (var line in new LineSplitter(info.Font, info.Scale, info.Text()).Split())
            {
                line.Measure();
                line.X = PixelScene.Align((w - line.Width) / 2);
                line.Y = PixelScene.Align(pos);
                Add(line);

                pos += line.Height;
            }

            Resize((int)w, (int)(pos + MARGIN));

            BadgeBanner.Highlight(icon, badge.Image);
        }
    }
}