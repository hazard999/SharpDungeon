namespace sharpdungeon.items.rings
{
    public class RingOfPower : Ring
    {
        public RingOfPower()
        {
            name = "Ring of Power";
        }

        protected internal override RingBuff Buff()
        {
            return new Power(this);
        }

        public override string Desc()
        {
            return IsKnown ? "Your wands will become more powerful in the energy field " + "that radiates from this ring. Degraded rings of power will instead weaken your wands." : base.Desc();
        }

        public class Power : RingBuff
        {
            public Power(Ring ring) : base(ring)
            {
            }
        }
    }
}