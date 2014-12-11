using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Slow : Weapon.Enchantment
    {
        private const string TxtChilling = "Chilling {0}";

        private static readonly ItemSprite.Glowing Blue = new ItemSprite.Glowing(0x0044FF);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 25%
            // lvl 1 - 40%
            // lvl 2 - 50%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 4) < 3) 
                return false;

            Buff.Prolong<actors.buffs.Slow>(defender, pdsharp.utils.Random.Float(1, 1.5f + level));

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Blue;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtChilling, weaponName);
        }
    }
}