using sharpdungeon.sprites;

namespace sharpdungeon.items.quest
{
    public class CorpseDust : Item
    {
        public CorpseDust()
        {
            name = "corpse dust";
            image = ItemSpriteSheet.DUST;

            cursed = true;
            cursedKnown = true;

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
            return "The ball of corpse dust doesn't differ outwardly from a regular dust ball. However, " + "you know somehow that it's better to Get rid of it as soon as possible.";
        }
    }
}