namespace sharpdungeon.items.rings
{
    public class RingOfSatiety : Ring
    {
        public RingOfSatiety()
        {
            name = "Ring of Satiety";
        }

        protected internal override RingBuff Buff()
        {
            return new Satiety(this);
        }

        public override string Desc()
        {
            return IsKnown ? "Wearing this ring you can go without food longer. Degraded rings of satiety will cause the opposite effect." : base.Desc();
        }

        public class Satiety : RingBuff
        {
            public Satiety(Ring ring) : base(ring)
            {
            }
        }
    }
}