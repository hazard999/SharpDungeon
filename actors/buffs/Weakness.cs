using sharpdungeon.actors.hero;
using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Weakness : FlavourBuff
	{
		private const float DURATION = 40f;

		public override int Icon()
		{
			return BuffIndicator.WEAKNESS;
		}

		public override string ToString()
		{
			return "Weakened";
		}

		public override bool AttachTo(Character target)
		{
		    if (base.AttachTo(target))
			{
				var hero = (Hero)target;
				hero.Weakened = true;
				hero.Belongings.Discharge();

				return true;
			}
		    
            return false;
		}

	    public override void Detach()
		{
			base.Detach();
			((Hero)Target).Weakened = false;
		}

		public static float Duration(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() * DURATION : DURATION;
		}
	}
}