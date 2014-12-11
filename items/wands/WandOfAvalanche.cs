using System;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.mechanics;
using sharpdungeon.utils;
using pdsharp.noosa;

namespace sharpdungeon.items.wands
{
    public class WandOfAvalanche : Wand
    {
        public WandOfAvalanche()
        {
            name = "Wand of Avalanche";
            HitChars = false;
        }

        protected internal override void OnZap(int cell)
        {
            Sample.Instance.Play(Assets.SND_ROCKS);

            var localLevel = Level;

            Ballistica.Distance = Math.Min(Ballistica.Distance, 8 + localLevel);

            var size = 1 + localLevel / 3;
            PathFinder.BuildDistanceMap(cell, BArray.not(levels.Level.solid, null), size);

            for (var i = 0; i < levels.Level.Length; i++)
            {
                int d = PathFinder.Distance[i];

                if (d >= int.MaxValue) 
                    continue;

                var ch = Actor.FindChar(i);
                if (ch != null)
                {
                    ch.Sprite.Flash();

                    ch.Damage(pdsharp.utils.Random.Int(2, 6 + (size - d) * 2), this);

                    if (ch.IsAlive && pdsharp.utils.Random.Int(2 + d) == 0)
                        Buff.Prolong<Paralysis>(ch, pdsharp.utils.Random.IntRange(2, 6));
                }

                CellEmitter.Get(i).Start(Speck.Factory(Speck.ROCK), 0.07f, 3 + (size - d));
                Camera.Main.Shake(3, 0.07f * (3 + (size - d)));
            }

            if (CurUser.IsAlive) 
                return;

            Dungeon.Fail(Utils.Format(ResultDescriptions.WAND, name, Dungeon.Depth));
            GLog.Negative("You killed yourself with your own Wand of Avalanche...");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Earth(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "When a discharge of this wand hits a wall (or any other solid obstacle) it causes " + "an avalanche of stones, damaging and stunning All creatures in the affected area.";
        }
    }
}