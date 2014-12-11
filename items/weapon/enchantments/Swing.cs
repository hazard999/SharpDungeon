using sharpdungeon.actors;
using sharpdungeon.levels;
using System;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Swing : Weapon.Enchantment
    {
        private const string TxtWild = "Wild {0}";

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, weapon.level);

            var maxDamage = (int)(damage * Math.Pow(2, -1d / (level + 1)));

            if (maxDamage < 1)
                return false;

            var p = attacker.pos;
            int[] neighbours = { p + 1, p - 1, p + Level.Width, p - Level.Width, p + 1 + Level.Width, p + 1 - Level.Width, p - 1 + Level.Width, p - 1 - Level.Width };

            foreach (var n in neighbours)
            {
                var ch = Actor.FindChar(n);
                if (ch == null || ch == defender || !ch.IsAlive)
                    continue;

                var dr = pdsharp.utils.Random.IntRange(0, ch.Dr());
                var dmg = pdsharp.utils.Random.Int(1, maxDamage);
                var effectiveDamage = Math.Max(dmg - dr, 0);

                ch.Damage(effectiveDamage, this);

                ch.Sprite.BloodBurstA(attacker.Sprite.Center(), effectiveDamage);
                ch.Sprite.Flash();
            }

            return true;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtWild, weaponName);
        }
    }
}