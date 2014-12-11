using System;
using System.Collections.Generic;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor.glyphs
{
    public class Multiplicity : Glyph
    {
        private const string TxtMultiplicity = "{0} of multiplicity";

        private static readonly ItemSprite.Glowing Pink = new ItemSprite.Glowing(0xCCAA88);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, armor.level);

            if (pdsharp.utils.Random.Int(level / 2 + 6) < 5)
                return damage;

            var respawnPoints = new List<int>();

            for (var i = 0; i < Level.NEIGHBOURS8.Length; i++)
            {
                var p = defender.pos + Level.NEIGHBOURS8[i];
                if (Actor.FindChar(p) == null && (Level.passable[p] || Level.avoid[p]))
                    respawnPoints.Add(p);
            }

            if (respawnPoints.Count <= 0)
                return damage;

            var mob = new MirrorImage();
            mob.Duplicate((Hero)defender);
            GameScene.Add(mob);
            WandOfBlink.Appear(mob, pdsharp.utils.Random.Element(respawnPoints));

            defender.Damage(pdsharp.utils.Random.IntRange(1, defender.HT / 6), this); //attacker
            CheckOwner(defender);

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtMultiplicity, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Pink;
        }
    }
}