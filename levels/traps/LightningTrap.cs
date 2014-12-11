using System;
using pdsharp.noosa;
using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.utils;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.levels.traps
{
    public class LightningTrap
    {
        private const string name = "lightning trap";

        // 00x66CCEE
        public static void Trigger(int pos, Character ch)
        {
            if (ch != null)
            {
                ch.Damage(Math.Max(1, Random.Int(ch.HP / 3, 2 * ch.HP / 3)), LIGHTNING);
                if (ch == Dungeon.Hero)
                {
                    Camera.Main.Shake(2, 0.3f);

                    if (!ch.IsAlive)
                    {
                        Dungeon.Fail(Utils.Format(ResultDescriptions.TRAP, name, Dungeon.Depth));
                        GLog.Negative("You were killed by a discharge of a lightning trap...");
                    }
                    else
                        ((Hero)ch).Belongings.Charge(false);
                }

                var points = new int[2];

                points[0] = pos - Level.Width;
                points[1] = pos + Level.Width;
                ch.Sprite.Parent.Add(new Lightning(points, 2, null));

                points[0] = pos - 1;
                points[1] = pos + 1;
                ch.Sprite.Parent.Add(new Lightning(points, 2, null));
            }

            CellEmitter.Center(pos).Burst(SparkParticle.Factory, Random.IntRange(3, 4));

        }

        public static readonly Electricity LIGHTNING = new Electricity();
        public class Electricity
        {
        }
    }
}