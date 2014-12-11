namespace sharpdungeon.items.rings
{
    public class RingOfHerbalism : Ring
    {
        public RingOfHerbalism()
        {
            name = "Ring of Herbalism";
        }

        protected internal override RingBuff Buff()
        {
            return new Herbalism(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring increases your chance to gather dew and seeds from trampled grass." : base.Desc();
        }

        public class Herbalism : RingBuff
        {
            public Herbalism(Ring ring) : base(ring)
            {
            }
        }
    }
}