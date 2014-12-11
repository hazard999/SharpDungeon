using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
	public class Shuriken : MissileWeapon
	{
	    public Shuriken()
	        : this(1)
	    {
	        name = "shuriken";
	        image = ItemSpriteSheet.SHURIKEN;

	        Str = 13;

	        Min = 2;
	        Max = 6;

	        Dly = 0.5f;
	    }

	    public Shuriken(int number)
	    {
			quantity = number;
		}

		public override string Desc()
		{
			return "Star-shaped pieces of metal with razor-sharp blades do significant damage " + 
                "when they hit a target. They can be thrown at very high rate.";
		}

		public override Item Random()
		{
			quantity = pdsharp.utils.Random.Int(5, 15);
			return this;
		}

		public override int Price()
		{
			return 15 * quantity;
		}
	}
}