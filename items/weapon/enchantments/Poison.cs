using sharpdungeon.actors;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Poison : Weapon.Enchantment
    {
        private const string TxtVenomous = "Venomous {0}";

        private static readonly ItemSprite.Glowing Purple = new ItemSprite.Glowing(0x4400AA);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 33%
            // lvl 1 - 50%
            // lvl 2 - 60%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 3) < 2)
                return false;

            actors.buffs.Buff.Affect<actors.buffs.Poison>(defender).Set(actors.buffs.Poison.DurationFactor(defender) * (level + 1));

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Purple;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtVenomous, weaponName);
        }
    }
}