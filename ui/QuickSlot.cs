using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors;
using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.windows;

namespace sharpdungeon.ui
{
    public class QuickSlot : Button, WndBag.Listener
    {
        private const string TxtSelectItem = "Select an item for the quickslot";

        private static QuickSlot _instance;

        private Item _itemInSlot;
        private ItemSlot slot;

        private Image _crossB;
        private Image _crossM;

        private bool _targeting;
        private Item _lastItem;
        private Character _lastTarget;

        public QuickSlot()
        {
            Item(Select());

            _instance = this;
        }

        public override void Destroy()
        {
            base.Destroy();

            _instance = null;

            _lastItem = null;
            _lastTarget = null;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            slot = new ItemSlot();
            slot.ClickAction = button =>
            {
                if (_targeting)
                    GameScene.HandleCell(_lastTarget.pos);
                else
                {
                    var item = Select();
                    if (item == _lastItem)
                        UseTargeting();
                    else
                        _lastItem = item;
                    item.Execute(Dungeon.Hero);
                }
            };

            slot.LongClickAction = button => OnLongClick();
            slot.TouchDownAction = button => slot.Icon.Lightness(0.7f);
            slot.TouchUpAction = button => slot.Icon.ResetColor();
            Add(slot);

            _crossB = Icons.TARGET.Get();
            _crossB.Visible = false;
            Add(_crossB);

            _crossM = new Image();
            _crossM.Copy(_crossB);
        }

        protected override void Layout()
        {
            base.Layout();

            slot.Fill(this);

            _crossB.X = PixelScene.Align(X + (Width - _crossB.Width) / 2);
            _crossB.Y = PixelScene.Align(Y + (Height - _crossB.Height) / 2);
        }

        protected override void OnClick()
        {
            GameScene.SelectItem(this, WndBag.Mode.QUICKSLOT, TxtSelectItem);
        }

        protected override bool OnLongClick()
        {
            GameScene.SelectItem(this, WndBag.Mode.QUICKSLOT, TxtSelectItem);
            return true;
        }

        private static Item Select()
        {
            if (Dungeon.Quickslot != null)
                return Dungeon.Quickslot;
            
            if (Dungeon.Quickslot != null)
            {
                var item = Dungeon.Hero.Belongings.GetItem(Dungeon.Quickslot);

                return item ?? items.Item.Virtual(Dungeon.Quickslot);
            }
            
            return null;
        }

        public void OnSelect(Item item)
        {
            if (item == null) 
                return;

            Dungeon.Quickslot = item;
            Refresh();
        }

        public virtual void Item(Item item)
        {
            slot.Item(item);
            _itemInSlot = item;
            EnableSlot();
        }

        public virtual void Enable(bool value)
        {
            Active = value;
            if (value)
                EnableSlot();
            else
                slot.Enable(false);
        }

        private void EnableSlot()
        {
            slot.Enable(_itemInSlot != null && _itemInSlot.Quantity() > 0 && (Dungeon.Hero.Belongings.Backpack.Contains(_itemInSlot) || _itemInSlot.IsEquipped(Dungeon.Hero)));
        }

        private void UseTargeting()
        {
            _targeting = _lastTarget != null && _lastTarget.IsAlive && Dungeon.Visible[_lastTarget.pos];

            if (!_targeting) 
                return;

            if (Actor.All.Contains(_lastTarget))
            {
                _lastTarget.Sprite.Parent.Add(_crossM);
                _crossM.Point(DungeonTilemap.TileToWorld(_lastTarget.pos));
                _crossB.Visible = true;
            }
            else
                _lastTarget = null;
        }

        public static void Refresh()
        {
            if (_instance != null)
                _instance.Item(Select());
        }

        public static void Target(Item item, Character target)
        {
            if (item != _instance._lastItem || target == Dungeon.Hero) 
                return;

            _instance._lastTarget = target;

            HealthIndicator.Instance.Target(target);
        }

        public static void Cancel()
        {
            if (_instance != null && _instance._targeting)
            {
                _instance._crossB.Visible = false;
                _instance._crossM.Remove();
                _instance._targeting = false;
            }
        }
    }
}