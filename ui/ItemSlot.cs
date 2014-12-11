using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.weapon;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.ui
{
    public class ItemSlot : Button
    {
        public const int Degraded = 0xFF4444;
        public const int Upgraded = 0x44FF44;
        public const int Warning = 0xFF8800;

        private const float Enabled = 1.0f;
        private const float Disabled = 0.3f;

        protected internal ItemSprite Icon;
        protected internal BitmapText TopLeft;
        protected internal BitmapText TopRight;
        protected internal BitmapText BottomRight;

        private const string TxtStrength = ":{0}";
        private const string TxtTypicalStr = "{0}?";

        private const string TxtLevel = "{0}";

        // Special items for containers
        public static Item Chest = new Item
        {
            image = ItemSpriteSheet.CHEST
        };

        public static Item LockedChest = new Item
        {
            image = ItemSpriteSheet.LOCKED_CHEST
        };

        public static Item Tomb = new Item { image = ItemSpriteSheet.TOMB };

        public static Item Skeleton = new Item { image = ItemSpriteSheet.BONES };

        public ItemSlot()
        {
        }

        public ItemSlot(Item _item)
            : this()
        {            
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            Icon = new ItemSprite();
            Add(Icon);

            TopLeft = new BitmapText(PixelScene.font1x);
            Add(TopLeft);

            TopRight = new BitmapText(PixelScene.font1x);
            Add(TopRight);

            BottomRight = new BitmapText(PixelScene.font1x);
            Add(BottomRight);
        }

        protected override void Layout()
        {
            base.Layout();

            Icon.X = X + (Width - Icon.Width) / 2;
            Icon.Y = Y + (Height - Icon.Height) / 2;

            if (TopLeft != null)
            {
                TopLeft.X = X;
                TopLeft.Y = Y;
            }

            if (TopRight != null)
            {
                TopRight.X = X + (Width - TopRight.Width);
                TopRight.Y = Y;
            }

            if (BottomRight != null)
            {
                BottomRight.X = Y + (Width - BottomRight.Width);
                BottomRight.Y = Y + (Height - BottomRight.Height);
            }
        }

        public virtual void Item(Item item)
        {
            if (item == null)
            {
                Active = false;
                Icon.Visible = TopLeft.Visible = TopRight.Visible = BottomRight.Visible = false;
            }
            else
            {
                Active = true;
                Icon.Visible = TopLeft.Visible = TopRight.Visible = BottomRight.Visible = true;

                Icon.View(item.Image, item.Glowing());

                TopLeft.Text(item.Status());

                var isArmor = item is Armor;
                var isWeapon = item is Weapon;
                if (isArmor || isWeapon)
                {
                    if (item.levelKnown || (isWeapon && !(item is MeleeWeapon)))
                    {
                        var str = isArmor ? ((Armor) item).Str : ((Weapon) item).Str;
                        TopRight.Text(Utils.Format(TxtStrength, str));
                        if (str > Dungeon.Hero.STR)
                            TopRight.Hardlight(Degraded);
                        else
                            TopRight.ResetColor();
                    }
                    else
                    {
                        TopRight.Text(Utils.Format(TxtTypicalStr,
                            isArmor ? ((Armor) item).TypicalStr() : ((MeleeWeapon) item).TypicalStr()));
                        TopRight.Hardlight(Warning);
                    }

                    TopRight.Measure();
                }
                else
                    TopRight.Text(null);

                var level = item.VisiblyUpgraded();

                if (level != 0 || (item.cursed && item.cursedKnown))
                {
                    BottomRight.Text(item.levelKnown ? Utils.Format(TxtLevel, level) : "");
                    BottomRight.Measure();
                    BottomRight.Hardlight(level > 0 ? Upgraded : Degraded);
                }
                else
                    BottomRight.Text(null);

                Layout();
            }
        }

        public virtual void Enable(bool value)
        {
            Active = value;

            var alpha = value ? Enabled : Disabled;
            Icon.Alpha(alpha);
            TopLeft.Alpha(alpha);
            TopRight.Alpha(alpha);
            BottomRight.Alpha(alpha);
        }

        public virtual void ShowParams(bool value)
        {
            if (value)
            {
                Add(TopRight);
                Add(BottomRight);
            }
            else
            {
                Remove(TopRight);
                Remove(BottomRight);
            }
        }
    }
}