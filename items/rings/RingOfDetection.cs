using sharpdungeon.actors.hero;

namespace sharpdungeon.items.rings
{
    public class RingOfDetection : Ring
    {
        public RingOfDetection()
        {
            name = "Ring of Detection";
        }

        public override bool DoEquip(Hero hero)
        {
            if (!base.DoEquip(hero)) 
                return false;

            Dungeon.Hero.Search(false);
            return true;
        }

        protected internal override RingBuff Buff()
        {
            return new Detection(this);
        }

        public override string Desc()
        {
            return IsKnown ? "Wearing this ring will allow the wearer to notice hidden secrets - " + "traps and secret doors - without taking time to search. Degraded rings of detection " + "will dull your senses, making it harder to notice secrets even when actively searching for them." : base.Desc();
        }

        public class Detection : RingBuff
        {
            public Detection(Ring ring) : base(ring)
            {
            }
        }
    }
}