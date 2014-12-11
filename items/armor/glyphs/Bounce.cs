using System;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.levels;

namespace sharpdungeon.items.armor.glyphs
{
    public class Bounce : Glyph
    {
        private const string TXT_BOUNCE = "{0} of bounce";

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, armor.level);

            if (!Level.Adjacent(attacker.pos, defender.pos) || pdsharp.utils.Random.Int(level + 5) < 4) 
                return damage;

            foreach (var neighbours in Level.NEIGHBOURS8)
            {
                if (attacker.pos - defender.pos != neighbours) 
                    continue;

                var newPos = attacker.pos + neighbours;
                if ((Level.passable[newPos] || Level.avoid[newPos]) && Actor.FindChar(newPos) == null)
                {
                    Actor.AddDelayed(new Pushing(attacker, attacker.pos, newPos), -1);

                    attacker.pos = newPos;
                    // FIXME
                    if (attacker is Mob)
                        Dungeon.Level.MobPress((Mob) attacker);
                    else
                        Dungeon.Level.Press(newPos, attacker);
                }
                break;
            }

            return damage;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TXT_BOUNCE, weaponName);
        }
    }
}