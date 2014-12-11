using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.actors.blobs
{
    public class Foliage : Blob
    {
        protected internal override void Evolve()
        {
            const int fromPos = Width + 1;
            const int to = Level.Length - Width - 1;

            var map = Dungeon.Level.map;
            var regrowth = false;

            var visible = false;

            for (var pos = fromPos; pos < to; pos++)
            {
                if (Cur[pos] > 0)
                {
                    Off[pos] = Cur[pos];
                    Volume += Off[pos];

                    if (map[pos] == Terrain.EMBERS)
                    {
                        map[pos] = Terrain.GRASS;
                        regrowth = true;
                    }

                    visible = visible || Dungeon.Visible[pos];
                }
                else
                    Off[pos] = 0;
            }

            var hero = Dungeon.Hero;
            if (hero.IsAlive && hero.VisibleEnemies == 0 && Cur[hero.pos] > 0)
                Buff.Affect<Shadows>(hero).Prolong();

            if (regrowth)
                GameScene.UpdateMap();

            if (visible)
                Journal.Add(Journal.Feature.GARDEN);
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);
            emitter.Start(ShaftParticle.Factory, 0.9f, 0);
        }

        public override string TileDesc()
        {
            return "Shafts of light pierce the gloom of the underground garden.";
        }
    }
}