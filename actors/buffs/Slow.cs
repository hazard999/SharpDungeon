using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Slow : FlavourBuff
    {
        private const float DURATION = 10f;

        public override int Icon()
        {
            return BuffIndicator.SLOW;
        }

        public override string ToString()
        {
            return "Slowed";
        }

        public static float Duration(Character ch)
        {
            var r = ch.Buff<RingOfElements.Resistance>();
            return r != null ? r.DurationFactor() * DURATION : DURATION;
        }
    }
}