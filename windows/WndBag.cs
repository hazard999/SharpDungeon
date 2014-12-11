using Android.Graphics;
using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.bags;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.plants;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndBag : WndTabbed
    {
        public enum Mode
        {
            ALL,
            UNIDENTIFED,
            UPGRADEABLE,
            QUICKSLOT,
            FOR_SALE,
            WEAPON,
            ARMOR,
            WAND,
            SEED
        }

        protected internal const int COLS = 4;

        protected internal const int SLOT_SIZE = 28;
        protected internal const int SLOT_MARGIN = 1;

        protected internal const int TAB_WIDTH = 30;

        protected internal const int TITLE_HEIGHT = 12;

        protected internal const int ROWS = (Belongings.BackpackSize + 4 + 1) / COLS + ((Belongings.BackpackSize + 4 + 1) % COLS > 0 ? 1 : 0);

        private Listener listener;
        private Mode mode;
        private string title;

        protected internal int count;
        protected internal int col;
        protected internal int row;

        private static Mode lastMode;
        private static Bag lastBag;

        public WndBag(Bag bag, Listener listener, Mode mode, string title)
        {
            this.listener = listener;
            this.mode = mode;
            this.title = title;

            lastMode = mode;
            lastBag = bag;

            var txtTitle = PixelScene.CreateText(title ?? Utils.Capitalize(bag.Name), 9);
            txtTitle.Hardlight(TitleColor);
            txtTitle.Measure();
            txtTitle.X = (int)(SLOT_SIZE * COLS + SLOT_MARGIN * (COLS - 1) - txtTitle.Width) / 2;
            txtTitle.Y = (int)(TITLE_HEIGHT - txtTitle.Height) / 2;
            Add(txtTitle);

            PlaceItems(bag);

            Resize(SLOT_SIZE * COLS + SLOT_MARGIN * (COLS - 1), SLOT_SIZE * ROWS + SLOT_MARGIN * (ROWS - 1) + TITLE_HEIGHT);

            var stuff = Dungeon.Hero.Belongings;
            Bag[] bags = { stuff.Backpack, stuff.GetItem<SeedPouch>(), stuff.GetItem<ScrollHolder>(), stuff.GetItem<WandHolster>() };

            foreach (var b in bags)
            {
                if (b == null)
                    continue;

                var tab = new BagTab(this, b);
                tab.SetSize(TAB_WIDTH, TabHeight());
                Add(tab);

                tab.Select(b == bag);
            }
        }

        public static WndBag LastBag(Listener listener, Mode mode, string title)
        {
            if (mode == lastMode && lastBag != null && Dungeon.Hero.Belongings.Backpack.Contains(lastBag))
                return new WndBag(lastBag, listener, mode, title);

            return new WndBag(Dungeon.Hero.Belongings.Backpack, listener, mode, title);
        }

        public static WndBag SeedPouch(Listener listener, Mode mode, string title)
        {
            var pouch = Dungeon.Hero.Belongings.GetItem<SeedPouch>();
            return pouch != null ? new WndBag(pouch, listener, mode, title) : new WndBag(Dungeon.Hero.Belongings.Backpack, listener, mode, title);
        }

        protected internal virtual void PlaceItems(Bag container)
        {
            // Equipped items
            var stuff = Dungeon.Hero.Belongings;

            if (stuff.Weapon != null)
                PlaceItem(stuff.Weapon);
            else
                PlaceItem(new Placeholder(ItemSpriteSheet.WEAPON));

            PlaceItem(stuff.Armor != null ? (Item)stuff.Armor : new Placeholder(ItemSpriteSheet.ARMOR));
            PlaceItem(stuff.Ring1 != null ? (Item)stuff.Ring1 : new Placeholder(ItemSpriteSheet.RING));
            PlaceItem(stuff.Ring2 != null ? (Item)stuff.Ring2 : new Placeholder(ItemSpriteSheet.RING));

            // Unequipped items
            foreach (var item in container.Items)
                PlaceItem(item);

            // Empty slots
            while (count - 4 < container.Size)
                PlaceItem(null);

            // Gold
            if (container != Dungeon.Hero.Belongings.Backpack)
                return;

            row = ROWS - 1;
            col = COLS - 1;
            PlaceItem(new Gold(Dungeon.Gold));
        }

        protected internal virtual void PlaceItem(Item item)
        {
            var x = col * (SLOT_SIZE + SLOT_MARGIN);
            var y = TITLE_HEIGHT + row * (SLOT_SIZE + SLOT_MARGIN);
            var itemButton = new ItemButton(this, item);
            itemButton.SetPos(x, y);            
            Add(itemButton);
            itemButton.Item(item);

            if (++col >= COLS)
            {
                col = 0;
                row++;
            }

            count++;
        }

        public override void OnMenuPressed()
        {
            if (listener == null)
                Hide();
        }

        public override void OnBackPressed()
        {
            if (listener != null)
                listener.OnSelect(null);

            base.OnBackPressed();
        }

        protected internal override void OnClick(Tab tab)
        {
            Hide();
            GameScene.Show(new WndBag(((BagTab)tab).Bag, listener, mode, title));
        }

        protected internal override int TabHeight()
        {
            return 20;
        }

        private class BagTab : Tab
        {
            private readonly Image _internalIcon;

            public readonly Bag Bag;

            public BagTab(WndTabbed owner, Bag bag)
                : base(owner)
            {
                Bag = bag;

                _internalIcon = Icon();
                Add(_internalIcon);
            }

            protected internal override void Select(bool value)
            {
                base.Select(value);
                _internalIcon.Am = Selected ? 1.0f : 0.6f;
            }

            protected override void Layout()
            {
                base.Layout();

                _internalIcon.Copy(Icon());
                _internalIcon.X = X + (Width - _internalIcon.Width) / 2;
                _internalIcon.Y = Y + (Height - _internalIcon.Height) / 2 - 2 - (Selected ? 0 : 1);

                if (Selected || !(_internalIcon.Y < Y + Cut))
                    return;

                var frame = _internalIcon.Frame();
                frame.Top += (Y + Cut - _internalIcon.Y) / _internalIcon.texture.Height;
                _internalIcon.Frame(frame);
                _internalIcon.Y = Y + Cut;
            }

            private Image Icon()
            {
                if (Bag is SeedPouch)
                    return Icons.SEED_POUCH.Get();

                if (Bag is ScrollHolder)
                    return Icons.SCROLL_HOLDER.Get();

                if (Bag is WandHolster)
                    return Icons.WAND_HOLSTER.Get();

                return Icons.BACKPACK.Get();
            }
        }

        private class Placeholder : Item
        {
            public Placeholder(int image)
            {
                name = null;
                this.image = image;
            }

            public override bool Identified
            {
                get
                {
                    return true;
                }
            }

            public override bool IsEquipped(Hero hero)
            {
                return true;
            }
        }

        private class ItemButton : ItemSlot
        {
            private readonly Color _normal = Color.Argb(0xFF, 0x4A, 0x4D, 0x44);
            private readonly Color _equipped = Color.Argb(0xFF, 0x63, 0x66, 0x5B);

            private readonly WndBag _owner;
            private Item _item;
            private ColorBlock bg;

            public ItemButton(WndBag owner, Item item)
                : base(item)
            {
                _owner = owner;
                _item = item;

                if (item is Gold)
                    bg.Visible = false;

                _Width = _Height = SLOT_SIZE;
            }

            protected override void CreateChildren()
            {
                bg = new ColorBlock(SLOT_SIZE, SLOT_SIZE, _normal);
                Add(bg);

                base.CreateChildren();
            }

            protected override void Layout()
            {
                bg.X = X;
                bg.Y = Y;

                base.Layout();
            }

            public override void Item(Item item)
            {
                base.Item(item);
                if (item != null)
                {
                    bg.Texture(TextureCache.CreateSolid(item.IsEquipped(Dungeon.Hero) ? _equipped : _normal));
                    if (item.cursed && item.cursedKnown)
                    {
                        bg.RA = +0.2f;
                        bg.Ga = -0.1f;
                    }
                    else if (!item.Identified)
                    {
                        bg.RA = 0.1f;
                        bg.Ba = 0.1f;
                    }

                    if (item.Name == null)
                        Enable(false);
                    else
                    {
                        Enable(_owner.mode == Mode.FOR_SALE && (item.Price() > 0) &&
                               (!item.IsEquipped(Dungeon.Hero) || !item.cursed) ||
                               _owner.mode == Mode.UPGRADEABLE && item.Upgradable ||
                               _owner.mode == Mode.UNIDENTIFED && !item.Identified ||
                               _owner.mode == Mode.QUICKSLOT && (item.DefaultAction != null) ||
                               _owner.mode == Mode.WEAPON && (item is MeleeWeapon || item is Boomerang) ||
                               _owner.mode == Mode.ARMOR && (item is Armor) ||
                               _owner.mode == Mode.WAND && (item is Wand) ||
                               _owner.mode == Mode.SEED && (item is Plant.Seed) || _owner.mode == Mode.ALL);
                    }
                }
                else
                    bg.Color(_normal);
            }

            protected override void OnTouchDown()
            {
                bg.Brightness(1.5f);
                Sample.Instance.Play(Assets.SND_CLICK, 0.7f, 0.7f, 1.2f);
            }

            protected override void OnTouchUp()
            {
                bg.Brightness(1.0f);
            }

            protected override void OnClick()
            {
                if (_owner.listener != null)
                {
                    _owner.Hide();
                    _owner.listener.OnSelect(_item);
                }
                else
                    Add(new WndItem(_owner, _item));
            }

            protected override bool OnLongClick()
            {
                if (_owner.listener == null && _item.DefaultAction != null)
                {
                    _owner.Hide();
                    Dungeon.Quickslot = _item;
                    QuickSlot.Refresh();
                    return true;
                }

                return false;
            }
        }

        public interface Listener
        {
            void OnSelect(Item item);
        }
    }
}