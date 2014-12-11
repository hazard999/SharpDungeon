using sharpdungeon.actors;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.sprites;
namespace sharpdungeon.items.armor.glyphs
{
    public class Displacement : Glyph
    {
        private const string TxtDisplacement = "{0} of displacement";

        private static readonly ItemSprite.Glowing Blue = new ItemSprite.Glowing(0x66AAFF);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            if (Dungeon.BossLevel())
                return damage;

            var nTries = (armor.level < 0 ? 1 : armor.level + 1) * 5;
            for (var i = 0; i < nTries; i++)
            {
                var pos = pdsharp.utils.Random.Int(Level.Length);

                if (!Dungeon.Visible[pos] || !Level.passable[pos] || Actor.FindChar(pos) != null)
                    continue;

                WandOfBlink.Appear(defender, pos);
                Dungeon.Level.Press(pos, defender);
                Dungeon.Observe();

                break;
            }

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtDisplacement, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Blue;
        }
    }
}