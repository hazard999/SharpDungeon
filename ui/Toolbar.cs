using System;
using System.Linq;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.windows;

namespace sharpdungeon.ui
{
    public class Toolbar : Component
    {
        private Tool _btnWait;
        private Tool _btnSearch;
        private Tool _btnInfo;
        private Tool _btnResume;
        private Tool _btnInventory;
        private Tool _btnQuick;

        private PickedUpItem _pickedUp;

        private bool _lastEnabled = true;

        public Toolbar()
        {
            _Height = _btnInventory.Height;
        }

        protected override void CreateChildren()
        {
            _btnWait = new Tool(0, 7, 20, 24);
            _btnWait.ClickAction = (button) => Dungeon.Hero.Rest(false);
            _btnWait.LongClickAction = (button) =>
            {
                Dungeon.Hero.Rest(true);
                return true;
            };

            Add(_btnWait);
            _btnSearch = new Tool(20, 7, 20, 24);
            _btnSearch.ClickAction = (button) => Dungeon.Hero.Search(true);
            Add(_btnSearch);

            _btnInfo = new Tool(40, 7, 21, 24);
            _btnInfo.ClickAction = (button) => GameScene.SelectCell(Informer);
            Add(_btnInfo);

            _btnResume = new Tool(61, 7, 21, 24);
            _btnResume.ClickAction = (button) => Dungeon.Hero.Resume();
            Add(_btnResume);

            _btnInventory = new Tool(82, 7, 23, 24);
            _btnInventory.ClickAction = (button) => GameScene.Show(new WndBag(Dungeon.Hero.Belongings.Backpack, null, WndBag.Mode.ALL, null));
            _btnInventory.LongClickAction = (button) =>
            {
                GameScene.Show(new WndCatalogus()); return true; 
            }; 
            
            //Override protected void createChildren() { base.createChildren(); gold = new GoldIndicator(); Add(gold); }; Override protected void layout() { base.layout(); gold.Fill(this); };
            Add(_btnInventory);

            Add(_btnQuick = new QuickslotTool(105, 7, 22, 24));

            Add(_pickedUp = new PickedUpItem());
        }

        protected override void Layout()
        {
            _btnWait.SetPos(X, Y);
            _btnSearch.SetPos(_btnWait.Right(), Y);
            _btnInfo.SetPos(_btnSearch.Right(), Y);
            _btnResume.SetPos(_btnInfo.Right(), Y);
            _btnQuick.SetPos(Width - _btnQuick.Width, Y);
            _btnInventory.SetPos(_btnQuick.Left() - _btnInventory.Width, Y);
        }

        public override void Update()
        {
            base.Update();

            if (_lastEnabled != Dungeon.Hero.ready)
            {
                _lastEnabled = Dungeon.Hero.ready;

                foreach (var localTool in Members.OfType<Tool>())
                    localTool.Enable(_lastEnabled);
            }

            _btnResume.Visible = Dungeon.Hero.lastAction != null;

            if (!Dungeon.Hero.IsAlive)
                _btnInventory.Enable(true);
        }

        public virtual void Pickup(Item item)
        {
            _pickedUp.Reset(item, _btnInventory.CenterX(), _btnInventory.CenterY());
        }

        private static readonly CellSelector.Listener Informer = new ToolbarCellSelectorListener();

        private class Tool : Button
        {
            private const int Bgcolor = 0x7B8073;

            private Image _baseImage;

            public Tool(int x, int y, int width, int height)
            {
                _baseImage.Frame(x, y, width, height);

                _Width = width;
                _Height = height;
            }


            protected override void CreateChildren()
            {
                base.CreateChildren();

                _baseImage = new Image(Assets.TOOLBAR);
                Add(_baseImage);
            }

            protected override void Layout()
            {
                base.Layout();

                _baseImage.X = X;
                _baseImage.Y = Y;
            }

            protected override void OnTouchDown()
            {
                _baseImage.Brightness(1.4f);
            }

            protected override void OnTouchUp()
            {
                if (IsActive)
                    _baseImage.ResetColor();
                else
                    _baseImage.Tint(Bgcolor, 0.7f);
            }

            public virtual void Enable(bool value)
            {
                if (value == IsActive)
                    return;

                if (value)
                    _baseImage.ResetColor();
                else
                    _baseImage.Tint(Bgcolor, 0.7f);

                IsActive = value;
            }
        }

        private class QuickslotTool : Tool
        {

            private QuickSlot _slot;

            public QuickslotTool(int x, int y, int width, int height)
                : base(x, y, width, height)
            {
            }

            protected override void CreateChildren()
            {
                base.CreateChildren();

                _slot = new QuickSlot();
                Add(_slot);
            }

            protected override void Layout()
            {
                base.Layout();
                _slot.SetRect(X + 1, Y + 2, Width - 2, Height - 2);
            }

            public override void Enable(bool value)
            {
                _slot.Enable(value);
                Active = value;
            }
        }

        private class PickedUpItem : ItemSprite
        {
            private const float Distance = DungeonTilemap.Size;
            private const float Duration = 0.2f;

            private float _dstX;
            private float _dstY;
            private float _left;

            public PickedUpItem()
            {
                OriginToCenter();

                Active = Visible = false;
            }

            public virtual void Reset(Item item, float dstX, float dstY)
            {
                View(item.Image, item.Glowing());

                Active = Visible = true;

                _dstX = dstX - SIZE / 2;
                _dstY = dstY - SIZE / 2;
                _left = Duration;

                X = _dstX - Distance;
                Y = _dstY - Distance;
                Alpha(1);
            }

            public override void Update()
            {
                base.Update();

                if ((_left -= Game.Elapsed) <= 0)
                    Visible = Active = false;
                else
                {
                    var p = _left / Duration;
                    Scale.Set((float)Math.Sqrt(p));
                    var offset = Distance * p;
                    X = _dstX - offset;
                    Y = _dstY - offset;
                }
            }
        }
    }

    internal class ToolbarCellSelectorListener : CellSelector.Listener
    {
        public void OnSelect(int? cellArg)
        {
            if (cellArg == null)
                return;

            var cell = cellArg.Value;

            
            if (cell < 0 || cell > Level.Length || (!Dungeon.Level.visited[cell] && !Dungeon.Level.mapped[cell]))
            {
                GameScene.Show(new WndMessage("You don't know what is there."));
                return;
            }

            if (!Dungeon.Visible[cell])
            {
                GameScene.Show(new WndInfoCell(cell));
                return;
            }

            if (cell == Dungeon.Hero.pos)
            {
                GameScene.Show(new WndHero());
                return;
            }

            var mob = (Mob)Actor.FindChar(cell);
            if (mob != null)
            {
                GameScene.Show(new WndInfoMob(mob));
                return;
            }

            var heap = Dungeon.Level.heaps[cell];
            if (heap != null)
            {
                if (heap.HeapType == Heap.Type.ForSale && heap.Size() == 1 && heap.Peek().Price() > 0)
                    GameScene.Show(new WndTradeItem(heap, false));
                else
                    GameScene.Show(new WndInfoItem(heap));

                return;
            }

            var plant = Dungeon.Level.plants[cell];
            if (plant != null)
            {
                GameScene.Show(new WndInfoPlant(plant));
                return;
            }

            GameScene.Show(new WndInfoCell(cell));
        }

        public string Prompt()
        {
            return "Select a cell to examine";
        }
    }
}