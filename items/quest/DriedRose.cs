using sharpdungeon.sprites;

namespace sharpdungeon.items.quest
{
    public class DriedRose : Item
    {
        public DriedRose()
        {
            name = "dried rose";
            image = ItemSpriteSheet.ROSE;

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
            return "The rose has dried long ago, but it has kept All its petals somehow.";
        }
    }
}