using System;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Paralysis : Weapon.Enchantment
    {
        private const string TxtStunning = "Stunning {0}";

        private static readonly ItemSprite.Glowing Yellow = new ItemSprite.Glowing(0xCCAA44);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 13%
            // lvl 1 - 22%
            // lvl 2 - 30%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 8) < 7) 
                return false;

            Buff.Prolong<actors.buffs.Paralysis>(defender, pdsharp.utils.Random.Float(1, 1.5f + level));

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Yellow;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtStunning, weaponName);
        }
    }
}