using sharpdungeon.items;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndInfoItem : Window
    {
        private const string TxtChest = "Chest";
        private const string TxtLockedChest = "Locked chest";
        private const string TxtCrystalChest = "Crystal chest";
        private const string TxtTomb = "Tomb";
        private const string TxtSkeleton = "Skeletal remains";
        private const string TxtWontKnow = "You won't know what's inside until you open it!";
        private const string TxtNeedKey = TxtWontKnow + " But to open it you need a golden key.";
        private const string TxtInside = "You can see {0} inside, but to open the chest you need a golden key.";
        private const string TxtOwner = "This ancient tomb may contain something useful, " + "but its owner will most certainly object to checking.";
        private const string TxtRemains = "This is All that's left from one of your predecessors. " + "Maybe it's worth checking for any valuables.";

        private const float Gap = 2;

        private const int WIDTH = 120;

        public WndInfoItem(Heap heap)
        {
            if (heap.HeapType == Heap.Type.Heap || heap.HeapType == Heap.Type.ForSale)
            {
                var item = heap.Peek();

                var color = TitleColor;
                if (item.levelKnown && item.level > 0)
                    color = ItemSlot.Upgraded;
                else
                    if (item.levelKnown && item.level < 0)
                        color = ItemSlot.Degraded;
                FillFields(item.image, item.Glowing(), color, item.ToString(), item.Info());

            }
            else
            {
                string title;
                string info;

                switch (heap.HeapType)
                {
                    case Heap.Type.Chest:
                        title = TxtChest;
                        info = TxtWontKnow;
                        break;
                    case Heap.Type.Tomb:
                        title = TxtTomb;
                        info = TxtOwner;
                        break;
                    case Heap.Type.Skeleton:
                        title = TxtSkeleton;
                        info = TxtRemains;
                        break;
                    case Heap.Type.CrystalChest:
                        title = TxtCrystalChest;
                        info = Utils.Format(TxtInside, Utils.Indefinite(heap.Peek().Name));
                        break;
                    default:
                        title = TxtLockedChest;
                        info = TxtNeedKey;
                        break;
                }

                FillFields(heap.Image(), heap.Glowing(), TitleColor, title, info);

            }
        }

        public WndInfoItem(Item item)
        {
            var color = TitleColor;
            if (item.levelKnown && item.level > 0)
                color = ItemSlot.Upgraded;
            else
                if (item.levelKnown && item.level < 0)
                    color = ItemSlot.Degraded;

            FillFields(item.image, item.Glowing(), color, item.ToString(), item.Info());
        }

        private void FillFields(int image, ItemSprite.Glowing glowing, int titleColor, string title, string info)
        {
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(image, glowing));
            titlebar.Label(Utils.Capitalize(title), titleColor);
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            var txtInfo = PixelScene.CreateMultiline(info, 6);
            txtInfo.MaxWidth = WIDTH;
            txtInfo.Measure();
            txtInfo.X = titlebar.Left();
            txtInfo.Y = titlebar.Bottom() + Gap;
            Add(txtInfo);

            Resize(WIDTH, (int)(txtInfo.Y + txtInfo.Height));
        }
    }
}