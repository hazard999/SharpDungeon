using System;
using System.Linq;
using sharpdungeon.actors.hero;
using sharpdungeon.items.rings;

namespace sharpdungeon.actors.buffs
{
    public class Regeneration : Buff
    {
        private const float RegenerationDelay = 10;

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                if (Target.HP < Target.HT && !((Hero)Target).IsStarving)
                    Target.HP += 1;

                var bonus =
                    Target.Buffs<RingOfMending.Rejuvenation>().Sum(buff => ((RingOfMending.Rejuvenation)buff).Level);

                Spend((float)(RegenerationDelay / Math.Pow(1.2, bonus)));
            }
            else
                Deactivate();

            return true;
        }
    }
}