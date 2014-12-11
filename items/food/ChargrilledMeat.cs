using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.food
{
    public class ChargrilledMeat : Food
    {
        public ChargrilledMeat()
        {
            name = "chargrilled meat";
            image = ItemSpriteSheet.STEAK;
            Energy = Hunger.Starving - Hunger.Hungry;
        }

        public override string Info()
        {
            return "It looks like a decent steak.";
        }

        public override int Price()
        {
            return 5 * quantity;
        }

        public static Food Cook(MysteryMeat ingredient)
        {
            var result = new ChargrilledMeat();
            result.quantity = ingredient.Quantity();
            return result;
        }
    }
}