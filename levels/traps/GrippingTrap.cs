using System;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.levels.traps
{
    public class GrippingTrap
    {
        public static void Trigger(int pos, Character c)
        {
            if (c != null)
            {
                var damage = Math.Max(0, (Dungeon.Depth + 3) - Random.IntRange(0, c.Dr()/2));
                Buff.Affect<Bleeding>(c).Set(damage);
                Buff.Prolong<Cripple>(c, Cripple.Duration);
                Wound.Hit(c);
            }
            else
                Wound.Hit(pos);
        }
    }
}