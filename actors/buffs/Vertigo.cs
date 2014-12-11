using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Vertigo : FlavourBuff
	{
		public const float DURATION = 10f;

		public override int Icon()
		{
			return BuffIndicator.VERTIGO;
		}

		public override string ToString()
		{
			return "Vertigo";
		}

		public static float Duration(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() * DURATION : DURATION;
		}
	}
}