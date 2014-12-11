using sharpdungeon.sprites;

namespace sharpdungeon.items.quest
{
    public class DarkGold : Item
    {
        public DarkGold()
        {
            name = "dark gold ore";
            image = ItemSpriteSheet.ORE;

            Stackable = true;
            unique = true;
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override string Info()
        {
            return "This metal is called dark not because of its color (it doesn't differ from the normal gold), " + "but because it melts under the daylight, making it useless on the surface.";
        }

        public override int Price()
        {
            return quantity;
        }
    }
}