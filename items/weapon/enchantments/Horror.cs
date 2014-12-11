using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Horror : Weapon.Enchantment
    {
        private const string TxtEldritch = "Eldritch {0}";

        private static readonly ItemSprite.Glowing Grey = new ItemSprite.Glowing(0x222222);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 20%
            // lvl 1 - 33%
            // lvl 2 - 43%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 5) < 4)
                return false;

            if (defender == Dungeon.Hero)
                Buff.Affect<Vertigo>(defender, Vertigo.Duration(defender));
            else
                Buff.Affect<Terror>(defender, Terror.Duration).Source = attacker;

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Grey;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtEldritch, weaponName);
        }
    }
}