using pdsharp.noosa.ui;
using sharpdungeon.items;

namespace sharpdungeon.ui
{
    public class LootIndicator : Tag
    {
        private ItemSlot _slot;

        private Item _lastItem;
        private int _lastQuantity;

        public LootIndicator()
            : base(0x1F75CC)
        {
            SetSize(24, 22);

            Visible = false;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();
            
            _slot = new ItemSlot();
            _slot.ClickAction = ItemSlotClickAction;
            _slot.ShowParams(false);
            Add(_slot);
        }

        private void ItemSlotClickAction(Button button)
        {
            Dungeon.Hero.Handle(Dungeon.Hero.pos);
        }

        protected override void Layout()
        {
            base.Layout();

            _slot.SetRect(X + 2, Y + 3, Width - 2, Height - 6);
        }

        public override void Update()
        {
            if (Dungeon.Hero.ready)
            {
                var heap = Dungeon.Level.heaps[Dungeon.Hero.pos];
                if (heap != null)
                {
                    var item = heap.HeapType == Heap.Type.Chest ? ItemSlot.Chest : heap.HeapType == Heap.Type.LockedChest ? ItemSlot.LockedChest : heap.HeapType == Heap.Type.Tomb ? ItemSlot.Tomb : heap.HeapType == Heap.Type.Skeleton ? ItemSlot.Skeleton : heap.Peek();
                    if (item != _lastItem || item.Quantity() != _lastQuantity)
                    {
                        _lastItem = item;
                        _lastQuantity = item.Quantity();

                        _slot.Item(item);
                        Flash();
                    }

                    Visible = true;

                }
                else
                {
                    _lastItem = null;
                    Visible = false;
                }
            }

            _slot.Enable(Visible && Dungeon.Hero.ready);

            base.Update();
        }
    }
}