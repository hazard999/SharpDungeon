using System;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.effects.particles;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Death : Weapon.Enchantment
    {
        private const string TxtGrim = "Grim {0}";

        private static readonly ItemSprite.Glowing Black = new ItemSprite.Glowing(0x000000);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 8%
            // lvl 1 ~ 9%
            // lvl 2 ~ 10%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 100) < 92) 
                return false;

            defender.Damage(defender.HP, this);
            defender.Sprite.Emitter().Burst(ShadowParticle.Up, 5);

            if (!defender.IsAlive && attacker is Hero)
                Badge.ValidateGrimWeapon();

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Black;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtGrim, weaponName);
        }
    }
}