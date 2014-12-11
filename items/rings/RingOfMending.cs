namespace sharpdungeon.items.rings
{
    public class RingOfMending : Ring
    {
        public RingOfMending()
        {
            name = "Ring of Mending";
        }

        protected internal override RingBuff Buff()
        {
            return new Rejuvenation(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring increases the body's regenerative properties, allowing " + "one to recover lost health at an accelerated rate. Degraded rings will " + "decrease or even halt one's natural regeneration." : base.Desc();
        }

        public class Rejuvenation : RingBuff
        {
            public Rejuvenation(Ring ring) : base(ring)
            {
            }
        }
    }
}