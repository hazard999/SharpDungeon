using sharpdungeon.sprites;

namespace sharpdungeon.items.quest
{
    public class RatSkull : Item
    {
        public RatSkull()
        {
            name = "giant rat skull";
            image = ItemSpriteSheet.SKULL;

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
            return "It could be a nice hunting trophy, but it smells too bad to place it on a wall.";
        }

        public override int Price()
        {
            return 100;
        }
    }
}