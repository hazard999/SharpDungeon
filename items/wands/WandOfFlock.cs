using System.Linq;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.effects;
using sharpdungeon.mechanics;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.scenes;

namespace sharpdungeon.items.wands
{
    public class WandOfFlock : Wand
    {
        public WandOfFlock()
        {
            name = "Wand of Flock";
        }

        protected internal override void OnZap(int cell)
        {
            var localLevel = Level;

            var n = localLevel + 2;

            if (Actor.FindChar(cell) != null && Ballistica.Distance > 2)
                cell = Ballistica.Trace[Ballistica.Distance - 2];

            var passable = BArray.or(levels.Level.passable, levels.Level.avoid, null);

            foreach (var actor in Actor.All.OfType<Character>())
                passable[(actor).pos] = false;

            PathFinder.BuildDistanceMap(cell, passable, n);
            var dist = 0;

            if (Actor.FindChar(cell) != null)
            {
                PathFinder.Distance[cell] = int.MaxValue;
                dist = 1;
            }

            float lifespan = localLevel + 3;

        sheepLabel:
            for (var i = 0; i < n; i++)
            {
                do
                {
                    for (var j = 0; j < levels.Level.Length; j++)
                    {
                        if (PathFinder.Distance[j] != dist) 
                            continue;

                        var sheep = new Sheep();
                        sheep.Lifespan = lifespan;
                        sheep.pos = j;
                        GameScene.Add(sheep);
                        Dungeon.Level.MobPress(sheep);

                        CellEmitter.Get(j).Burst(Speck.Factory(Speck.WOOL), 4);

                        PathFinder.Distance[j] = int.MaxValue;

                        goto sheepLabel;
                    }
                    dist++;
                } while (dist < n);
            }
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Wool(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "A flick of this wand summons a flock of magic sheep, creating temporary impenetrable obstacle.";
        }

        public class Sheep : NPC
        {
            private static readonly string[] Quotes = { "Baa!", "Baa?", "Baa.", "Baa..." };

            public Sheep()
            {
                Name = "sheep";
                SpriteClass = typeof (SheepSprite);
            }

            public float Lifespan;

            private bool _initialized;

            protected override bool Act()
            {
                if (_initialized)
                {
                    HP = 0;

                    Destroy();
                    Sprite.DoDie();
                }
                else
                {
                    _initialized = true;
                    Spend(Lifespan + pdsharp.utils.Random.Float(2));
                }

                return true;
            }

            public override void Damage(int dmg, object src)
            {
            }

            public override string Description()
            {
                return "This is a magic sheep. What's so magical about it? You can't kill it. " + "It will stand there until it magcially fades away, All the while chewing cud with a blank stare.";
            }

            public override void Interact()
            {
                Yell(pdsharp.utils.Random.Element(Quotes));
            }

            public override bool Reset()
            {
                return true;
            }
        }
    }
}