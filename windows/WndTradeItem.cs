using pdsharp.noosa.ui;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.rings;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndTradeItem : Window
    {
        private readonly Heap _heap;
        private const float Gap = 2;
        private const int WIDTH = 120;
        private const int BtnHeight = 18;

        private const string TxtSale = "FOR SALE: {0} - {1}g";
        private const string TxtBuy = "Buy for {0}g";
        private const string TxtSell = "Sell for {0}g";
        private const string TxtSell1 = "Sell 1 for {0}g";
        private const string TxtSellAll = "Sell All for {0}g";
        private const string TxtCancel = "Never mind";

        private const string TxtSold = "You've sold your {0} for {0}g";
        private const string TxtBought = "You've bought {0} for {0}g";

        private readonly Shopkeeper _keeper;
        private readonly Item _item;
        private readonly WndBag _owner;

        public WndTradeItem(Shopkeeper keeper, Item item, WndBag owner)
        {
            _keeper = keeper;
            _item = item;
            _owner = owner;

            var pos = CreateDescription(item, false);

            if (item.Quantity() == 1)
            {
                var btnSell = new RedButton(Utils.Format(TxtSell, item.Price()));
                btnSell.ClickAction = SellAction;
                btnSell.SetRect(0, pos + Gap, WIDTH, BtnHeight);
                Add(btnSell);

                pos = btnSell.Bottom();
            }
            else
            {
                var priceAll = item.Price();
                var btnSell1 = new RedButton(Utils.Format(TxtSell1, priceAll / item.Quantity()));
                btnSell1.ClickAction = SellOneAction;
                btnSell1.SetRect(0, pos + Gap, WIDTH, BtnHeight);
                Add(btnSell1);

                var btnSellAll = new RedButton(Utils.Format(TxtSellAll, priceAll));
                btnSellAll.ClickAction = SellAction;
                btnSellAll.SetRect(0, btnSell1.Bottom() + Gap, WIDTH, BtnHeight);
                Add(btnSellAll);

                pos = btnSellAll.Bottom();
            }

            var btnCancel = new RedButton(TxtCancel);
            btnCancel.ClickAction = CancelAction;
            btnCancel.SetRect(0, pos + Gap, WIDTH, BtnHeight);
            Add(btnCancel);

            Resize(WIDTH, (int)btnCancel.Bottom());
        }

        private void CancelAction(Button obj)
        {
            Hide();
        }

        private void SellOneAction(Button obj)
        {
            SellOne(_item);
            Hide();
        }

        private void SellAction(Button obj)
        {
            Sell(_item);
            Hide();
        }

        public WndTradeItem(Heap heap, bool canBuy)
        {
            _heap = heap;
            var item = heap.Peek();

            var pos = CreateDescription(item, true);

            var price = Price(item);

            if (canBuy)
            {
                var btnBuy = new RedButton(Utils.Format(TxtBuy, price));
                btnBuy.ClickAction = BuyAction;
                btnBuy.SetRect(0, pos + Gap, WIDTH, BtnHeight);
                btnBuy.Enable(price <= Dungeon.Gold);
                Add(btnBuy);

                var btnCancel = new RedButton(TxtCancel);
                btnCancel.ClickAction = CancelAction;
                btnCancel.SetRect(0, btnBuy.Bottom() + Gap, WIDTH, BtnHeight);
                Add(btnCancel);

                Resize(WIDTH, (int)btnCancel.Bottom());
            }
            else
                Resize(WIDTH, (int)pos);
        }

        private void BuyAction(Button obj)
        {
            Hide();
            Buy(_heap);
        }

        public override void Hide()
        {
            base.Hide();

            if (_owner == null)
                return;

            _owner.Hide();
            _keeper.Sell();
        }

        private float CreateDescription(Item item, bool forSale)
        {
            // Title
            var titlebar = new IconTitle();
            titlebar.Icon(new ItemSprite(item.Image, item.Glowing()));
            titlebar.Label(forSale ? Utils.Format(TxtSale, item.ToString(), Price(item)) : Utils.Capitalize(item.ToString()));
            titlebar.SetRect(0, 0, WIDTH, 0);
            Add(titlebar);

            // Upgraded / degraded
            if (item.levelKnown && item.level > 0)
                titlebar.Color(ItemSlot.Upgraded);
            else
                if (item.levelKnown && item.level < 0)
                    titlebar.Color(ItemSlot.Degraded);

            // Description
            var info = PixelScene.CreateMultiline(item.Info(), 6);
            info.MaxWidth = WIDTH;
            info.Measure();
            info.X = titlebar.Left();
            info.Y = titlebar.Bottom() + Gap;
            Add(info);

            return info.Y + info.Height;
        }

        private void Sell(Item item)
        {
            var hero = Dungeon.Hero;

            if (item.IsEquipped(hero) && !((EquipableItem)item).DoUnequip(hero, false))
                return;

            item.DetachAll(hero.Belongings.Backpack);

            var price = item.Price();

            new Gold(price).DoPickUp(hero);
            GLog.Information(TxtSold, item.Name, price);
        }

        private void SellOne(Item item)
        {
            if (item.Quantity() <= 1)
                Sell(item);
            else
            {
                var hero = Dungeon.Hero;

                item = item.Detach(hero.Belongings.Backpack);
                var price = item.Price();

                new Gold(price).DoPickUp(hero);
                GLog.Information(TxtSold, item.Name, price);
            }
        }

        private int Price(Item item)
        {
            // This formula is not completely correct...
            var price = item.Price() * 5 * (Dungeon.Depth / 5 + 1);

            if (Dungeon.Hero.Buff<RingOfHaggler.Haggling>() != null && price >= 2)
                price /= 2;

            return price;
        }

        private void Buy(Heap heap)
        {
            var hero = Dungeon.Hero;
            var item = heap.PickUp();

            var price = Price(item);
            Dungeon.Gold -= price;

            GLog.Information(TxtBought, item.Name, price);

            if (!item.DoPickUp(hero))
                Dungeon.Level.Drop(item, heap.Pos).Sprite.Drop();
        }
    }
}