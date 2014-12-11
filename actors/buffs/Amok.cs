using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Amok : FlavourBuff
	{
		public override int Icon()
		{
			return BuffIndicator.AMOK;
		}

		public override string ToString()
		{
			return "Amok";
		}
	}
}