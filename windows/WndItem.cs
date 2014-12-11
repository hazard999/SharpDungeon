using System;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndItem : Window
    {
        private const float ButtonWidth = 36;
        private const float ButtonHeight = 16;

        private const float Gap = 2;

        private const int WIDTH = 120;

        public WndItem(WndBag owner, Item item)
        {

            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(item.image, item.Glowing()));
            titlebar.Label(Utils.Capitalize(item.ToString()));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            if (item.levelKnown && item.level > 0)
                titlebar.Color(ItemSlot.Upgraded);
            else
                if (item.levelKnown && item.level < 0)
                    titlebar.Color(ItemSlot.Degraded);

            var info = PixelScene.CreateMultiline(item.Info(), 6);
            info.MaxWidth = WIDTH;
            info.Measure();
            info.X = titlebar.Left();
            info.Y = titlebar.Bottom() + Gap;
            Add(info);

            var y = info.Y + info.Height + Gap;
            float x = 0;

            if (Dungeon.Hero.IsAlive && owner != null)
            {
                foreach (var action in item.Actions(Dungeon.Hero))
                {
                    var btn = new RedButton(action);
                    btn.ClickAction = button =>
                    {
                        item.Execute(Dungeon.Hero, action);
                        Hide();
                        owner.Hide();
                    };
                    btn.SetSize(Math.Max(ButtonWidth, btn.ReqWidth()), ButtonHeight);
                    if (x + btn.Width > WIDTH)
                    {
                        x = 0;
                        y += ButtonHeight + Gap;
                    }
                    btn.SetPos(x, y);
                    Add(btn);

                    x += btn.Width + Gap;
                }
            }

            Resize(WIDTH, (int)(y + (x > 0 ? ButtonHeight : 0)));
        }
    }
}