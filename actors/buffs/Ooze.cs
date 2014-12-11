using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.actors.buffs
{
    public class Ooze : Buff
    {
        private const string TXT_HERO_KILLED = "{0} killed you...";

        public int Damage = 1;

        public override int Icon()
        {
            return BuffIndicator.OOZE;
        }

        public override string ToString()
        {
            return "Caustic ooze";
        }

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                Target.Damage(Damage, this);
                if (!Target.IsAlive && Target == Dungeon.Hero)
                {
                    Dungeon.Fail(Utils.Format(ResultDescriptions.OOZE, Dungeon.Depth));
                    GLog.Negative(TXT_HERO_KILLED, ToString());
                }
                Spend(Tick);
            }

            if (levels.Level.water[Target.pos])
                Detach();
            return true;
        }
    }
}