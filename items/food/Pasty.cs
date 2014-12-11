using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.food
{
    public class Pasty : Food
    {
        public Pasty()
        {
            name = "pasty";
            image = ItemSpriteSheet.PASTY;
            Energy = Hunger.Starving;
        }

        public override string Info()
        {
            return "This is authentic Cornish pasty with traditional filling of beef and potato.";
        }

        public override int Price()
        {
            return 20 * Quantity();
        }
    }
}