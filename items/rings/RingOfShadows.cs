namespace sharpdungeon.items.rings
{
    public class RingOfShadows : Ring
    {
        public RingOfShadows()
        {
            name = "Ring of Shadows";
        }

        protected internal override RingBuff Buff()
        {
            return new Shadows(this);
        }

        public override string Desc()
        {
            return IsKnown ? "Enemies will be less likely to notice you if you wear this ring. Degraded rings " + "of shadows will alert enemies who might otherwise not have noticed your presence." : base.Desc();
        }

        public class Shadows : RingBuff
        {
            public Shadows(Ring ring) : base(ring)
            {
            }
        }
    }
}