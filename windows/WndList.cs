using System;
using System.Collections.Generic;
using pdsharp.noosa;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndList : Window
    {
        private const int LocalWidth = 120;
        private const int Margin = 4;
        private const int Gap = 4;

        private const string Dot = "\u007F";

        public WndList(IList<string> items)
        {
            float pos = Margin;
            float dotWidth = 0;
            float maxWidth = 0;

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                    pos += Gap;

                var dot = PixelScene.CreateText(Dot, 6);
                dot.X = Margin;
                dot.Y = pos;

                if (Math.Abs(dotWidth) < 0.0001)
                {
                    dot.Measure();
                    dotWidth = dot.Width;
                }

                Add(dot);

                var item = PixelScene.CreateMultiline(items[i], 6);
                item.X = dot.X + dotWidth;
                item.Y = pos;
                item.MaxWidth = (int)(LocalWidth - Margin * 2 - dotWidth);
                item.Measure();
                Add(item);

                pos += item.Height;
                var w = item.Width;
                if (w > maxWidth)
                    maxWidth = w;
            }

            Resize((int)(maxWidth + dotWidth + Margin * 2), (int)(pos + Margin));
        }
    }
}