using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Cripple : FlavourBuff
	{
		public const float Duration = 10f;

		public override int Icon()
		{
			return BuffIndicator.CRIPPLE;
		}

		public override string ToString()
		{
			return "Crippled";
		}
	}
}