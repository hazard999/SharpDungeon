using sharpdungeon.actors;
using sharpdungeon.levels;
using System;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Piercing : Weapon.Enchantment
    {
        private const string TxtPiercing = "Piercing {0}";

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, weapon.level);

            var maxDamage = (int)(damage * Math.Pow(2, -1d / (level + 1)));
            
            if (maxDamage < 1) 
                return false;

            var d = defender.pos - attacker.pos;
            var pos = defender.pos + d;

            do
            {
                var ch = Actor.FindChar(pos);
                if (ch == null)
                    break;

                var dr = pdsharp.utils.Random.IntRange(0, ch.Dr());
                var dmg = pdsharp.utils.Random.Int(1, maxDamage);
                var effectiveDamage = Math.Max(dmg - dr, 0);

                ch.Damage(effectiveDamage, this);

                ch.Sprite.BloodBurstA(attacker.Sprite.Center(), effectiveDamage);
                ch.Sprite.Flash();

                pos += d;
            } while (pos >= 0 && pos < Level.Length);

            return true;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtPiercing, weaponName);
        }
    }
}