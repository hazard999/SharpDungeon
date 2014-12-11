namespace sharpdungeon.items.rings
{
    public class RingOfEvasion : Ring
    {
        public RingOfEvasion()
        {
            name = "Ring of Evasion";
        }

        protected internal override RingBuff Buff()
        {
            return new Evasion(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring increases your chance to dodge enemy DoAttack." : base.Desc();
        }

        public class Evasion : RingBuff
        {
            public Evasion(Ring ring) : base(ring)
            {
            }
        }
    }
}