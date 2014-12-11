using sharpdungeon.actors;
using System.Collections.Generic;
using System.Linq;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels.traps;
using sharpdungeon.utils;
using pdsharp.noosa;
using pdsharp.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfLightning : Wand
    {
        public WandOfLightning()
        {
           name = "Wand of Lightning";
        }

        private readonly List<Character> _affected = new List<Character>();

        private readonly int[] _points = new int[20];
        private int _nPoints;

        protected internal override void OnZap(int cell)
        {
            if (CurUser.IsAlive) 
                return;

            Dungeon.Fail(Utils.Format(ResultDescriptions.WAND, name, Dungeon.Depth));
            GLog.Negative("You killed yourself with your own Wand of Lightning...");
        }

        private void Hit(Character ch, int damage)
        {
            while (true)
            {
                if (damage < 1)
                    return;

                if (ch == Dungeon.Hero)
                    Camera.Main.Shake(2, 0.3f);

                _affected.Add(ch);
                ch.Damage(levels.Level.water[ch.pos] && !ch.Flying ? damage*2 : damage, LightningTrap.LIGHTNING);

                ch.Sprite.CenterEmitter().Burst(SparkParticle.Factory, 3);
                ch.Sprite.Flash();

                _points[_nPoints++] = ch.pos;

                var ns = levels.Level.NEIGHBOURS8.Select(t => Actor.FindChar(ch.pos + t)).Where(n => n != null && !_affected.Contains(n)).ToList();

                if (ns.Count > 0)
                {
                    ch = pdsharp.utils.Random.Element(ns);
                    damage = pdsharp.utils.Random.Int(damage/2, damage);
                    continue;
                }

                break;
            }
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            _nPoints = 0;
            _points[_nPoints++] = Dungeon.Hero.pos;

            var ch = Actor.FindChar(cell);
            if (ch != null)
            {
                _affected.Clear();
                var lvl = Level;
                Hit(ch, pdsharp.utils.Random.Int(5 + lvl / 2, 10 + lvl));
            }
            else
            {
                _points[_nPoints++] = cell;
                CellEmitter.Center(cell).Burst(SparkParticle.Factory, 3);
            }
            
            CurUser.Sprite.Parent.Add(new Lightning(_points, _nPoints, callback));
        }

        public override string Desc()
        {
            return "This wand conjures forth deadly arcs of electricity, which deal damage " + "to several creatures standing close to each other.";
        }
    }
}