using System;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects.particles;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Fire : Weapon.Enchantment
    {
        private const string TxtBlazing = "Blazing {0}";

        private static readonly ItemSprite.Glowing Orange = new ItemSprite.Glowing(0xFF4400);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            // lvl 0 - 33%
            // lvl 1 - 50%
            // lvl 2 - 60%
            var level = Math.Max(0, weapon.level);

            if (pdsharp.utils.Random.Int(level + 3) < 2) 
                return false;

            if (pdsharp.utils.Random.Int(2) == 0)
                Buff.Affect<Burning>(defender).Reignite(defender);

            defender.Damage(pdsharp.utils.Random.Int(1, level + 2), this);

            defender.Sprite.Emitter().Burst(FlameParticle.Factory, level + 1);

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Orange;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtBlazing, weaponName);
        }
    }
}