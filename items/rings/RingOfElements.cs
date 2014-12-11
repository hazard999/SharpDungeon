using System;
using System.Collections.Generic;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs;
using sharpdungeon.levels.traps;

namespace sharpdungeon.items.rings
{
    public class RingOfElements : Ring
    {
        public RingOfElements()
        {
            name = "Ring of Elements";
        }

        protected internal override RingBuff Buff()
        {
            return new Resistance(this);
        }

        public override string Desc()
        {
            return IsKnown ? "This ring provides resistance to different elements, such as fire, " + "electricity, gases etc. Also it decreases duration of negative effects." : base.Desc();
        }

        private static readonly HashSet<Type> EMPTY = new HashSet<Type>();

        private static readonly HashSet<Type> FULL = new HashSet<Type>()
        {
            typeof (Burning),
            typeof (ToxicGas),
            typeof (Poison),
            typeof (LightningTrap.Electricity),
            typeof (Warlock),
            typeof (Eye),
            typeof (Yog.BurningFist)
        };

        public class Resistance : RingBuff
        {
            public Resistance(Ring ring) : base(ring)
            {
            }

            public virtual HashSet<Type> Resistances()
            {
                if (pdsharp.utils.Random.Int(Level + 3) >= 3)
                    return FULL;

                return EMPTY;
            }

            public virtual float DurationFactor()
            {
                return Level < 0 ? 1 : (2 + 0.5f * Level) / (2 + Level);
            }
        }
    }
}