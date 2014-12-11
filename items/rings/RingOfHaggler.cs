using sharpdungeon.actors.hero;

namespace sharpdungeon.items.rings
{
    public class RingOfHaggler : Ring
    {
        public RingOfHaggler()
        {
            name = "Ring of Haggler";
        }

        protected internal override RingBuff Buff()
        {
            return new Haggling(this);
        }

        public override Item Random()
        {
            level = +1;
            return this;
        }

        public override bool DoPickUp(Hero hero)
        {
            Identify();
            Badge.ValidateRingOfHaggler();
            Badge.ValidateItemLevelAquired(this);
            return base.DoPickUp(hero);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override string Desc()
        {
            return IsKnown ? "In fact this ring doesn't provide any magic effect, but it demonstrates " + "to shopkeepers and vendors, that the owner of the ring is a member of " + "The Thieves' Guild. Usually they are glad to give a discount in exchange " + "for temporary immunity guarantee. Upgrading this ring won't give any additional " + "bonuses." : base.Desc();
        }

        public class Haggling : RingBuff
        {
            public Haggling(Ring ring) : base(ring)
            {
            }
        }
    }
}