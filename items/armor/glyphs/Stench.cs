using System;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor.glyphs
{
    public class Stench : Glyph
    {
        private const string TxtStench = "{0} of stench";

        private static readonly ItemSprite.Glowing Green = new ItemSprite.Glowing(0x22CC44);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, armor.level);

            if (Level.Adjacent(attacker.pos, defender.pos) && pdsharp.utils.Random.Int(level + 5) >= 4)
                GameScene.Add(Blob.Seed(attacker.pos, 20, typeof(ToxicGas)));

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtStench, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Green;
        }
    }
}