using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.ui;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using sharpdungeon.utils;
using System;

namespace sharpdungeon.windows
{
    public class WndBlacksmith : Window
    {
        private const int BtnSize = 36;
        private const float Gap = 2;
        private const float BtnGap = 10;
        private const int WIDTH = 116;

        public ItemButton BtnPressed;
        public readonly ItemButton BtnItem1;
        public readonly ItemButton BtnItem2;
        public readonly RedButton BtnReforge;

        private const string TxtPrompt = "Ok, a deal is a deal, dat's what I can do for you: I can reforge " + "2 items and turn them into one of a better quality.";
        private const string TxtSelect = "Select an item to reforge";
        private const string TxtReforge = "Reforge them";

        public WndBlacksmith(Blacksmith troll, Hero hero)
        {
            ItemSelector = new BlacksmithItemSelector(this);

            var titlebar = new IconTitle();
            titlebar.Icon(troll.Sprite);
            titlebar.Label(Utils.Capitalize(troll.Name));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var message = PixelScene.CreateMultiline(TxtPrompt, 6);
            message.MaxWidth = WIDTH;
            message.Measure();
            message.Y = titlebar.Bottom() + Gap;
            Add(message);

            BtnItem1 = new ItemButton();
            BtnItem1.ClickAction = () =>
            {
                BtnPressed = BtnItem1;
                GameScene.SelectItem(ItemSelector, WndBag.Mode.UPGRADEABLE, TxtSelect);
            };
            BtnItem1.SetRect((WIDTH - BtnGap) / 2 - BtnSize, message.Y + message.Height + BtnGap, BtnSize, BtnSize);
            Add(BtnItem1);

            BtnItem2 = new ItemButton();
            BtnItem2.ClickAction = () =>
            {
                BtnPressed = BtnItem2;
                GameScene.SelectItem(ItemSelector, WndBag.Mode.UPGRADEABLE, TxtSelect);
            };
            BtnItem2.SetRect(BtnItem1.Right() + BtnGap, BtnItem1.Top(), BtnSize, BtnSize);
            Add(BtnItem2);

            BtnReforge = new RedButton(TxtReforge);
            BtnReforge.ClickAction = button =>
            {
                troll.Upgrade(BtnItem1.Item, BtnItem2.Item);
                Hide();
            };
            BtnReforge.Enable(false);
            BtnReforge.SetRect(0, BtnItem1.Bottom() + BtnGap, WIDTH, 20);
            Add(BtnReforge);


            Resize(WIDTH, (int)BtnReforge.Bottom());
        }

        protected WndBag.Listener ItemSelector;
        
        public class ItemButton : Component
        {
            protected internal NinePatch Bg;
            protected internal ItemSlot Slot;

            public Item Item;

            protected override void CreateChildren()
            {
                base.CreateChildren();

                Bg = sharpdungeon.Chrome.Get(sharpdungeon.Chrome.Type.BUTTON);
                Add(Bg);

                Slot = new ItemSlot();
                Slot.TouchDownAction = button =>
                {
                    Bg.Brightness(1.2f);
                    Sample.Instance.Play(Assets.SND_CLICK);
                };
                Slot.TouchUpAction = button => Bg.ResetColor();
                Slot.ClickAction = button => OnClick();
                Add(Slot);
            }

            public Action ClickAction { get; set; }

            protected internal virtual void OnClick()
            {
                if (ClickAction != null)
                    ClickAction();
            }

            protected override void Layout()
            {
                base.Layout();

                Bg.X = X;
                Bg.Y = Y;
                
                Bg.Size(Width, Height);

                Slot.SetRect(X + 2, Y + 2, Width - 4, Height - 4);
            }

            public virtual void item(Item item)
            {
                Slot.Item(Item = item);
            }
        }
    }

    public class BlacksmithItemSelector : WndBag.Listener
    {
        private readonly WndBlacksmith _wndBlacksmith;

        public BlacksmithItemSelector(WndBlacksmith wndBlacksmith)
        {
            _wndBlacksmith = wndBlacksmith;
        }

        public void OnSelect(Item item)
        {
            if (item == null) 
                return;

            _wndBlacksmith.BtnPressed.item(item);

            if (_wndBlacksmith.BtnItem1.Item == null || _wndBlacksmith.BtnItem2.Item == null) 
                return;

            var result = Blacksmith.Verify(_wndBlacksmith.BtnItem1.Item, _wndBlacksmith.BtnItem2.Item);
            if (result != null)
            {
                GameScene.Show(new WndMessage(result));
                _wndBlacksmith.BtnReforge.Enable(false);
            }
            else
                _wndBlacksmith.BtnReforge.Enable(true);
        }
    }
}