using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Charm : FlavourBuff
	{
        public override bool AttachTo(Character target)
        {
            if (!base.AttachTo(target)) 
                return false;

            target.Pacified = true;
            
            return true;
        }

        public override void Detach()
		{
			Target.Pacified = false;
			base.Detach();
		}

		public override int Icon()
		{
			return BuffIndicator.HEART;
		}

		public override string ToString()
		{
			return "Charmed";
		}

		public static float durationFactor(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() : 1;
		}
	}

}