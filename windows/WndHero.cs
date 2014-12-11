using System;
using System.Globalization;
using Java.Util;
using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors.buffs;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndHero : WndTabbed
    {

        private const string TXT_STATS = "Stats";
        private const string TXT_BUFFS = "Buffs";

        private const string TXT_EXP = "Experience";
        private const string TXT_STR = "Strength";
        private const string TXT_HEALTH = "Health";
        private const string TXT_GOLD = "Gold Collected";
        private const string TXT_DEPTH = "Maximum Depth";

        private const int WIDTH = 100;
        private const int TAB_WIDTH = 40;

        private StatsTab stats;
        private BuffsTab buffs;

        private SmartTexture icons;
        private TextureFilm film;

        public WndHero()
        {
            icons = TextureCache.Get(Assets.BUFFS_LARGE);
            film = new TextureFilm(icons, 16, 16);

            stats = new StatsTab(this);
            Add(stats);

            buffs = new BuffsTab(this);
            Add(buffs);

            var statsTab = new LabeledTab(this, TXT_STATS);
            statsTab.SelectAction = StatsSelect;
            Add(statsTab);

            var buffsTab = new LabeledTab(this, TXT_BUFFS);
            buffsTab.SelectAction = BuffsSelect;
            Add(buffsTab);

            foreach (var tab in Tabs)
                tab.SetSize(TAB_WIDTH, TabHeight());

            Resize(WIDTH, (int)Math.Max(stats.Height(), buffs.Height()));

            Select(0);
        }

        private void BuffsSelect(Tab obj)
        {
            buffs.Visible = buffs.Active = obj.Selected;
        }

        private void StatsSelect(Tab obj)
        {
            stats.Visible = stats.Active = obj.Selected;
        }

        private class StatsTab : Group
        {
            private readonly WndHero _wndHero;
            private const string TXT_TITLE = "Level {0} {1}";
            private const string TXT_CATALOGUS = "Catalogus";
            private const string TXT_JOURNAL = "Journal";

            private const int GAP = 5;

            private float pos;

            public StatsTab(WndHero wndHero)
            {
                _wndHero = wndHero;
                var hero = Dungeon.Hero;

                var title = PixelScene.CreateText(Utils.Format(TXT_TITLE, hero.Lvl, hero.ClassName()).ToUpper(), 9);
                title.Hardlight(TitleColor);
                title.Measure();
                Add(title);

                var btnCatalogus = new RedButton(TXT_CATALOGUS);
                btnCatalogus.ClickAction = CatalogusClickAction;
                btnCatalogus.SetRect(0, title.Y + title.Height, btnCatalogus.ReqWidth() + 2, btnCatalogus.ReqHeight() + 2);
                Add(btnCatalogus);

                var btnJournal = new RedButton(TXT_JOURNAL);
                btnJournal.ClickAction = JournalClickAction;
                btnJournal.SetRect(btnCatalogus.Right() + 1, btnCatalogus.Top(), btnJournal.ReqWidth() + 2, btnJournal.ReqHeight() + 2);
                Add(btnJournal);

                pos = btnCatalogus.Bottom() + GAP;

                StatSlot(TXT_STR, hero.STR);
                StatSlot(TXT_HEALTH, hero.HP + "/" + hero.HT);
                StatSlot(TXT_EXP, hero.Exp + "/" + hero.MaxExp());

                pos += GAP;

                StatSlot(TXT_GOLD, Statistics.GoldCollected);
                StatSlot(TXT_DEPTH, Statistics.DeepestFloor);

                pos += GAP;
            }

            private void CatalogusClickAction(Button button)
            {
                _wndHero.Hide();
                GameScene.Show(new WndCatalogus());
            }

            private void JournalClickAction(Button button)
            {
                _wndHero.Hide();
                GameScene.Show(new WndJournal());
            }

            private void StatSlot(string label, string value)
            {
                var txt = PixelScene.CreateText(label, 8);
                txt.Y = pos;
                Add(txt);

                txt = PixelScene.CreateText(value, 8);
                txt.Measure();
                txt.X = PixelScene.Align(WIDTH * 0.65f);
                txt.Y = pos;
                Add(txt);

                pos += GAP + txt.BaseLine();
            }

            private void StatSlot(string label, int value)
            {
                StatSlot(label, value.ToString(CultureInfo.InvariantCulture));
            }

            public virtual float Height()
            {
                return pos;
            }
        }

        private class BuffsTab : Group
        {
            private readonly WndHero _heroWindow;
            private const int Gap = 2;

            private float _pos;

            public BuffsTab(WndHero heroWindow)
            {
                _heroWindow = heroWindow;
                foreach (var buff in Dungeon.Hero.Buffs())
                    BuffSlot(buff);
            }

            private void BuffSlot(Buff buff)
            {
                var index = buff.Icon();

                if (index == BuffIndicator.NONE)
                    return;

                var icon = new Image(_heroWindow.icons);
                icon.Frame(_heroWindow.film.Get(index));
                icon.Y = _pos;
                Add(icon);

                var txt = PixelScene.CreateText(buff.ToString(), 8);
                txt.X = icon.Width + Gap;
                txt.Y = _pos + (int)(icon.Height - txt.BaseLine()) / 2;
                Add(txt);

                _pos += Gap + icon.Height;
            }

            public virtual float Height()
            {
                return _pos;
            }
        }
    }
}