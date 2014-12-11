using sharpdungeon.plants;
using sharpdungeon.sprites;

namespace sharpdungeon.items.bags
{
    public class SeedPouch : Bag
    {
        public SeedPouch()
        {
            name = "seed pouch";
            image = ItemSpriteSheet.POUCH;

            Size = 8;
        }

        public override bool Grab(Item item)
        {
            return item is Plant.Seed;
        }

        public override int Price()
        {
            return 50;
        }

        public override string Info()
        {
            return "This small velvet pouch allows you to store any number of seeds in it. Very convenient.";
        }
    }
}