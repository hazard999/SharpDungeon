using sharpdungeon.sprites;

namespace sharpdungeon.items
{
    public class Ankh : Item
    {
        public Ankh()
        {
            Stackable = true;
            name = "Ankh";
            image = ItemSpriteSheet.ANKH;
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
            return "The ancient symbol of immortality grants an ability to return to life after death. " + "Upon resurrection All non-equipped items are lost.";
        }

        public override int Price()
        {
            return 50 * Quantity();
        }
    }
}