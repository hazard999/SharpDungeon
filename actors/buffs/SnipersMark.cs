using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class SnipersMark : FlavourBuff
    {
        public override int Icon()
        {
            return BuffIndicator.MARK;
        }

        public override string ToString()
        {
            return "Sniper's mark";
        }
    }
}