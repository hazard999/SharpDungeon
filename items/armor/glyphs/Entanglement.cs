using System;
using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.plants;
using sharpdungeon.sprites;
using EarthParticle = sharpdungeon.effects.particles.EarthParticle;

namespace sharpdungeon.items.armor.glyphs
{
    public class Entanglement : Glyph
    {
        private const string TxtEntanglement = "{0} of entanglement";

        private static readonly ItemSprite.Glowing Green = new ItemSprite.Glowing(0x448822);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, armor.level);

            if (pdsharp.utils.Random.Int(4) != 0)
                return damage;

            Buff.Prolong<Roots>(defender, 5 - level / 5);
            Buff.Affect<Earthroot.Armor>(defender).Level = 5 * (level + 1);
            CellEmitter.Bottom(defender.pos).Start(EarthParticle.Factory, 0.05f, 8);
            Camera.Main.Shake(1, 0.4f);

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtEntanglement, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Green;
        }
    }
}