using sharpdungeon.effects;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfIdentify : InventoryScroll
    {
        public ScrollOfIdentify()
        {
            name = "Scroll of Identify";
            InventoryTitle = "Select an item to identify";
            Mode = WndBag.Mode.UNIDENTIFED;
        }

        protected internal override void OnItemSelected(Item item)
        {
            CurUser.Sprite.Parent.Add(new Identification(CurUser.Sprite.Center().Offset(0, -16)));

            item.Identify();
            GLog.Information("It is " + item);

            Badge.ValidateItemLevelAquired(item);
        }

        public override string Desc()
        {
            return "Permanently reveals All of the secrets of a single item.";
        }

        public override int Price()
        {
            return IsKnown ? 30 * Quantity() : base.Price();
        }
    }
}