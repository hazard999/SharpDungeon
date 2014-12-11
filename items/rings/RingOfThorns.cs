using sharpdungeon.actors.hero;
namespace sharpdungeon.items.rings
{
    public class RingOfThorns : Ring
    {
        public RingOfThorns()
        {
            name = "Ring of Thorns";
        }

        protected internal override RingBuff Buff()
        {
            return new Thorns(this);
        }

        public override Item Random()
        {
            level = +1;
            return this;
        }

        public override bool DoPickUp(Hero hero)
        {
            Identify();
            Badge.ValidateRingOfThorns();
            Badge.ValidateItemLevelAquired(this);
            return base.DoPickUp(hero);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override string Desc()
        {
            return IsKnown ? "Though this ring doesn't provide real thorns, an enemy that attacks you " + "will itself be wounded by a fraction of the damage that it inflicts. " + "Upgrading this ring won't give any additional bonuses." : base.Desc();
        }

        public class Thorns : RingBuff
        {
            public Thorns(Ring ring) : base(ring)
            {
            }
        }
    }
}