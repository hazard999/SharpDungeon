using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Fury : Buff
	{
		public static float Level = 0.4f;

	    protected override bool Act()
		{
	        if (Target.HP > Target.HT*Level)
	            Detach();

	        Spend(Tick);

			return true;
		}

		public override int Icon()
		{
			return BuffIndicator.FURY;
		}

		public override string ToString()
		{
			return "Fury";
		}
	}

}