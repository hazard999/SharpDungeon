using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Paralysis : FlavourBuff
	{
		private const float DURATION = 10f;

		public override bool AttachTo(Character target)
		{
		    if (!base.AttachTo(target)) 
                return false;

		    target.Paralysed = true;
		    return true;
		}

	    public override void Detach()
		{
			Target.Paralysed = false;
			base.Detach();
		}

		public override int Icon()
		{
			return BuffIndicator.PARALYSIS;
		}

		public override string ToString()
		{
			return "Paralysed";
		}

		public static float Duration(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() * DURATION : DURATION;
		}
	}
}