using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.food
{
	public class OverpricedRation : Food
	{
	    public OverpricedRation()
	    {
            name = "overpriced food ration";
            image = ItemSpriteSheet.OVERPRICED;
            Energy = Hunger.Starving - Hunger.Hungry;
            Message = "That food tasted ok.";   
	    }

		public override string Info()
		{
			return "It looks exactly like a standard ration of food but smaller.";
		}

		public override int Price()
		{
			return 20 * Quantity();
		}
	}
}