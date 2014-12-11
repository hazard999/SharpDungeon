using pdsharp.noosa;
using sharpdungeon.actors.hero;
using sharpdungeon.scenes;
using sharpdungeon.utils;
using System;

namespace sharpdungeon.windows
{
    public class WndClass : WndTabbed
    {
        private const string TxtMastery = "Mastery";

        private const int WIDTH = 110;

        private const int TabWidth = 50;

        public WndClass(HeroClass cl)
        {
            var tabPerks = new PerksTab(cl);
            Add(tabPerks);

            Tab tab = new RankingTab(this, Utils.Capitalize(cl.Title()), tabPerks);
            tab.SetSize(TabWidth, TabHeight());
            Add(tab);

            if (Badge.IsUnlocked(cl.MasteryBadge()))
            {
                var tabMastery = new MasteryTab(cl);
                Add(tabMastery);

                tab = new RankingTab(this, TxtMastery, tabMastery);
                tab.SetSize(TabWidth, TabHeight());
                Add(tab);

                Resize((int) Math.Max(tabPerks.Width, tabMastery.Width), (int) Math.Max(tabPerks.Height, tabMastery.Height));
            }
            else
                Resize((int) tabPerks.Width, (int) tabPerks.Height);

            Select(0);
        }

        private class RankingTab : LabeledTab
        {
            private readonly Group _page;

            public RankingTab(WndTabbed owner, string label, Group page)
                : base(owner, label)
            {
                _page = page;
            }

            protected internal override void Select(bool value)
            {
                base.Select(value);

                if (_page != null)
                    _page.Visible = _page.Active = Selected;
            }
        }

        private class PerksTab : Group
        {

            private const int MARGIN = 4;
            private const int GAP = 4;

            private const string DOT = "\u007F";

            public float Height { get; private set; }
            public float Width { get; private set; }

            public PerksTab(HeroClass cl)
            {
                float dotWidth = 0;
                
                var items = cl.Perks();
                float pos = MARGIN;

                for (var i = 0; i < items.Length; i++)
                {
                    if (i > 0)
                        pos += GAP;

                    var dot = PixelScene.CreateText(DOT, 6);
                    dot.X = MARGIN;
                    dot.Y = pos;
                    if (dotWidth == 0)
                    {
                        dot.Measure();
                        dotWidth = dot.Width;
                    }
                    Add(dot);

                    var item = PixelScene.CreateMultiline(items[i], 6);
                    item.X = dot.X + dotWidth;
                    item.Y = pos;
                    item.MaxWidth = (int)(WIDTH - MARGIN * 2 - dotWidth);
                    item.Measure();
                    Add(item);

                    pos += item.Height;
                    var w = item.Width;
                    if (w > Width)
                        Width = w;
                }

                Width += MARGIN + dotWidth;
                Height = pos + MARGIN;
            }
        }

        private class MasteryTab : Group
        {
            private const int MARGIN = 4;

            public float Height;
            public float Width;

            public MasteryTab(HeroClass cl)
            {
                string text = null;
                switch (cl.Ordinal())
                {
                    case HeroClassType.Warrior:
                        text = HeroSubClass.GLADIATOR.Desc + "\\Negative\\Negative" + HeroSubClass.BERSERKER.Desc;
                        break;
                    case HeroClassType.Mage:
                        text = HeroSubClass.BATTLEMAGE.Desc + "\\Negative\\Negative" + HeroSubClass.WARLOCK.Desc;
                        break;
                    case HeroClassType.Rogue:
                        text = HeroSubClass.FREERUNNER.Desc + "\\Negative\\Negative" + HeroSubClass.ASSASSIN.Desc;
                        break;
                    case HeroClassType.Huntress:
                        text = HeroSubClass.SNIPER.Desc + "\\Negative\\Negative" + HeroSubClass.WARDEN.Desc;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var hl = new Highlighter(text);

                var normal = PixelScene.CreateMultiline(hl.Text, 6);
                normal.MaxWidth = WIDTH - MARGIN * 2;
                normal.Measure();
                normal.X = MARGIN;
                normal.Y = MARGIN;
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

                Height = normal.Y + normal.Height + MARGIN;
                Width = normal.X + normal.Width + MARGIN;
            }
        }
    }
}