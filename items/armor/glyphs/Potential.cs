using System;
using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.levels.traps;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor.glyphs
{
    public class Potential : Glyph
    {
        private const string TxtPotential = "{0} of potential";

        private static readonly ItemSprite.Glowing Blue = new ItemSprite.Glowing(0x66CCEE);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, armor.level);

            if (!Level.Adjacent(attacker.pos, defender.pos) || pdsharp.utils.Random.Int(level + 7) < 6)
                return damage;

            var dmg = pdsharp.utils.Random.IntRange(1, damage);
            attacker.Damage(dmg, LightningTrap.LIGHTNING);
            dmg = pdsharp.utils.Random.IntRange(1, dmg);
            defender.Damage(dmg, LightningTrap.LIGHTNING);

            CheckOwner(defender);
            if (defender == Dungeon.Hero)
                Camera.Main.Shake(2, 0.3f);

            int[] points = { attacker.pos, defender.pos };
            attacker.Sprite.Parent.Add(new Lightning(points, 2, null));

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtPotential, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Blue;
        }
    }
}