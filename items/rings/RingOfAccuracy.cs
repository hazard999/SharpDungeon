namespace sharpdungeon.items.rings
{
    public class RingOfAccuracy : Ring
    {
        public RingOfAccuracy()
        {
            name = "Ring of Accuracy";
        }

        protected internal override RingBuff Buff()
        {
            return new Accuracy(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring increases your chance to hit the enemy." : base.Desc();
        }

        public class Accuracy : RingBuff
        {
            public Accuracy(Ring ring) : base(ring)
            {
            }
        }
    }
}