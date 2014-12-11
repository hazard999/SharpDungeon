using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.actors.buffs
{
    public class Combo : Buff
    {
        private const string TxtCombo = "{0} hit combo!";

        public int Count;

        public override int Icon()
        {
            return BuffIndicator.COMBO;
        }

        public override string ToString()
        {
            return "Combo";
        }

        public virtual int Hit(Character enemy, int damage)
        {
            Count++;

            if (Count >= 3)
            {
                Badge.ValidateMasteryCombo(Count);

                GLog.Positive(TxtCombo, Count);
                Postpone(1.41f - Count / 10f);
                return (int)(damage * (Count - 2) / 5f);
            }

            Postpone(1.1f);
            return 0;
        }

        protected override bool Act()
        {
            Detach();
            return true;
        }
    }
}