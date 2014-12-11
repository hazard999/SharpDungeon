using sharpdungeon.items.scrolls;
using sharpdungeon.sprites;

namespace sharpdungeon.items.bags
{
    public class ScrollHolder : Bag
    {
        public ScrollHolder()
        {
            name = "scroll holder";
            image = ItemSpriteSheet.HOLDER;

            Size = 12;
        }

        public override bool Grab(Item item)
        {
            return item is Scroll;
        }

        public override int Price()
        {
            return 50;
        }

        public override string Info()
        {
            return "You can place any number of scrolls into this tubular container. " + "It saves room in your backpack and protects scrolls from fire.";
        }
    }
}