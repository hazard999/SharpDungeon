namespace sharpdungeon.items.rings
{
    public class RingOfHaste : Ring
    {
        public RingOfHaste()
        {
            name = "Ring of Haste";
        }

        protected internal override RingBuff Buff()
        {
            return new Haste(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring accelerates the wearer's flow of time, allowing one to perform All actions a little faster." : base.Desc();
        }

        public class Haste : RingBuff
        {
            public Haste(Ring ring) : base(ring)
            {
            }
        }
    }
}