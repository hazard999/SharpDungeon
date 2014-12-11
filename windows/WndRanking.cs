using System;
using System.Globalization;
using Android.Graphics;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndRanking : WndTabbed
    {
        private const string TXT_ERROR = "Unable to load additional information";

        private const string TXT_STATS = "Stats";
        private const string TXT_ITEMS = "Items";
        private const string TXT_BADGES = "Badges";

        private const int WIDTH = 112;
        private const int HEIGHT = 144;

        private const int TAB_WIDTH = 40;

        //private Thread _thread;
        private string _error;

        private readonly Image _busy;

        public WndRanking(string gameFile)
        {
            Resize(WIDTH, HEIGHT);

            //var operation = new ParameterizedThreadStart(obj => RunThread(gameFile));
            //_thread = new Thread(operation);
            //_thread.Start();

            _busy = Icons.BUSY.Get();
            _busy.Origin.Set(_busy.Width / 2, _busy.Height / 2);
            _busy.AngularSpeed = 720;
            _busy.X = (WIDTH - _busy.Width) / 2;
            _busy.Y = (HEIGHT - _busy.Height) / 2;
            Add(_busy);
        }

        private void RunThread(string gameFile)
        {
            try
            {
                Badge.LoadGlobal();
                Dungeon.LoadGame(gameFile);
            }
            catch (Exception)
            {
                _error = TXT_ERROR;
            }
        }

        public override void Update()
        {
            base.Update();

            //if (_thread == null || _thread.IsAlive)
            //    return;

            //_thread = null;
            if (_error == null)
            {
                Remove(_busy);
                CreateControls();
            }
            else
            {
                Hide();
                Game.Scene.Add(new WndError(TXT_ERROR));
            }
        }

        private void CreateControls()
        {

            string[] labels = { TXT_STATS, TXT_ITEMS, TXT_BADGES };
            Group[] pages = { new StatsTab(), new ItemsTab(), new BadgesTab(this) };

            for (var i = 0; i < pages.Length; i++)
            {
                Add(pages[i]);

                Tab tab = new RankingTab(this, labels[i], pages[i]);
                tab.SetSize(TAB_WIDTH, TabHeight());
                Add(tab);
            }

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

        private class StatsTab : Group
        {
            private const int Gap = 4;

            private const string TxtTitle = "Level {0} {1}";

            private const string TxtChallenges = "Challenges";

            private const string TxtHealth = "Health";
            private const string TxtStr = "Strength";

            private const string TxtDuration = "Game Duration";

            private const string TxtDepth = "Maximum Depth";
            private const string TxtEnemies = "Mobs Killed";
            private const string TxtGold = "Gold Collected";

            private const string TxtFood = "Food Eaten";
            private const string TxtAlchemy = "Potions Cooked";
            private const string TxtAnkhs = "Ankhs Used";

            public StatsTab()
            {
                var heroClass = Dungeon.Hero.ClassName();

                var title = new IconTitle();
                title.Icon(HeroSprite.Avatar(Dungeon.Hero.heroClass, Dungeon.Hero.Tier()));
                title.Label(Utils.Format(TxtTitle, Dungeon.Hero.Lvl, heroClass).ToUpper(CultureInfo.CurrentCulture));
                title.SetRect(0, 0, WIDTH, 0);
                Add(title);

                float pos = title.Bottom();

                if (Dungeon.Challenges > 0)
                {
                    var btnCatalogus = new RedButton(TxtChallenges);
                    btnCatalogus.ClickAction = button => Game.Scene.Add(new WndChallenges(Dungeon.Challenges, false));
                    btnCatalogus.SetRect(0, pos + Gap, btnCatalogus.ReqWidth() + 2, btnCatalogus.ReqHeight() + 2);
                    Add(btnCatalogus);

                    pos = btnCatalogus.Bottom();
                }

                pos += Gap + Gap;

                pos = StatSlot(this, TxtStr, Dungeon.Hero.STR.ToString(), pos);
                pos = StatSlot(this, TxtHealth, Dungeon.Hero.HT.ToString(), pos);

                pos += Gap;

                pos = StatSlot(this, TxtDuration, ((int)Statistics.Duration).ToString(), pos);

                pos += Gap;

                pos = StatSlot(this, TxtDepth, Statistics.DeepestFloor.ToString(), pos);
                pos = StatSlot(this, TxtEnemies, Statistics.EnemiesSlain.ToString(), pos);
                pos = StatSlot(this, TxtGold, Statistics.GoldCollected.ToString(), pos);

                pos += Gap;

                pos = StatSlot(this, TxtFood, Statistics.FoodEaten.ToString(), pos);
                pos = StatSlot(this, TxtAlchemy, Statistics.PotionsCooked.ToString(), pos);
                pos = StatSlot(this, TxtAnkhs, Statistics.AnkhsUsed.ToString(), pos);
            }

            private float StatSlot(Group parent, string label, string value, float pos)
            {
                var txt = PixelScene.CreateText(label, 7);
                txt.Y = pos;
                parent.Add(txt);

                txt = PixelScene.CreateText(value, 7);
                txt.Measure();
                txt.X = PixelScene.Align(WIDTH * 0.65f);
                txt.Y = pos;
                parent.Add(txt);

                return pos + Gap + txt.BaseLine();
            }
        }

        private class ItemsTab : Group
        {
            private float _pos;

            public ItemsTab()
            {
                var stuff = Dungeon.Hero.Belongings;
                if (stuff.Weapon != null)
                    AddItem(stuff.Weapon);

                if (stuff.Armor != null)
                    AddItem(stuff.Armor);
                if (stuff.Ring1 != null)
                    AddItem(stuff.Ring1);
                if (stuff.Ring2 != null)
                    AddItem(stuff.Ring2);

                if (Dungeon.Quickslot is Item && Dungeon.Hero.Belongings.Backpack.Contains(Dungeon.Quickslot))
                    AddItem(Dungeon.Quickslot);
                //else
                //    if (Dungeon.Quickslot is Type)
                //{
                //    //ORIGINAL LINE: @SuppressWarnings("unchecked") Item item = Dungeon.hero.Belongings.getItem((Class<? extends Item>)Dungeon.quickslot);

                //    Item item = Dungeon.Hero.Belongings.GetItem(Type)Dungeon.Quickslot);
                //    if (item != null)
                //    {
                //        AddItem(item);
                //    }
                //}
            }

            private void AddItem(Item item)
            {
                var slot = new ItemButton(item);
                slot.SetRect(0, _pos, slot.Width, ItemButton.HEIGHT);
                Add(slot);

                _pos += slot.Height + 1;
            }
        }

        private class BadgesTab : Group
        {
            public BadgesTab(WndRanking wndRanking)
            {
                Camera = wndRanking.Camera;

                ScrollPane list = new BadgesList(false);
                Add(list);

                list.SetSize(WIDTH, HEIGHT);
            }
        }

        private class ItemButton : Button
        {

            public const int HEIGHT = 28;

            private readonly Item _item;

            private ItemSlot _slot;
            private ColorBlock _bg;
            private BitmapText _name;

            public ItemButton(Item item)
            {
                _item = item;

                _slot.Item(item);
                if (item.cursed && item.cursedKnown)
                {
                    _bg.RA = +0.2f;
                    _bg.Ga = -0.1f;
                }
                else if (!item.Identified)
                {
                    _bg.RA = 0.1f;
                    _bg.Ba = 0.1f;
                }
            }

            protected override void CreateChildren()
            {
                _bg = new ColorBlock(HEIGHT, HEIGHT, Color.Argb(0xFF, 0x4A, 0x4D, 0x44));
                Add(_bg);

                _slot = new ItemSlot();
                Add(_slot);

                _name = PixelScene.CreateText("?", 7);
                Add(_name);

                base.CreateChildren();
            }

            protected override void Layout()
            {
                _bg.X = X;
                _bg.Y = Y;

                _slot.SetRect(X, Y, HEIGHT, HEIGHT);

                _name.X = _slot.Right() + 2;
                _name.Y = Y + (Height - _name.BaseLine()) / 2;

                var str = Utils.Capitalize(_item.Name);
                _name.Text(str);
                _name.Measure();
                if (_name.Width > Width - _name.X)
                {
                    do
                    {
                        str = str.Substring(0, str.Length - 1);
                        _name.Text(str + "...");
                        _name.Measure();
                    }
                    while (_name.Width > Width - _name.X);
                }

                base.Layout();
            }

            protected override void OnTouchDown()
            {
                _bg.Brightness(1.5f);
                Sample.Instance.Play(Assets.SND_CLICK, 0.7f, 0.7f, 1.2f);
            }

            protected override void OnTouchUp()
            {
                _bg.Brightness(1.0f);
            }

            protected override void OnClick()
            {
                Game.Scene.Add(new WndItem(null, _item));
            }
        }
    }
}