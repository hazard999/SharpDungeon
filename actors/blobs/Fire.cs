using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.actors.blobs
{
    public class Fire : Blob
    {
        protected internal override void Evolve()
        {
            var flamable = Level.flamable;

            const int fromPos = Width + 1;
            const int to = Level.Length - Width - 1;

            var observe = false;

            for (var pos = fromPos; pos < to; pos++)
            {
                int fire;

                if (Cur[pos] > 0)
                {
                    Burn(pos);

                    fire = Cur[pos] - 1;
                    if (fire <= 0 && flamable[pos])
                    {
                        var oldTile = Dungeon.Level.map[pos];
                        Level.Set(pos, Terrain.EMBERS);

                        observe = true;
                        GameScene.UpdateMap(pos);
                        if (Dungeon.Visible[pos])
                            GameScene.DiscoverTile(pos, oldTile);
                    }
                }
                else
                {
                    if (flamable[pos] &&
                        (Cur[pos - 1] > 0 || Cur[pos + 1] > 0 || Cur[pos - Width] > 0 || Cur[pos + Width] > 0))
                    {
                        fire = 4;
                        Burn(pos);
                    }
                    else
                        fire = 0;
                }

                Volume += (Off[pos] = fire);

            }

            if (observe)
                Dungeon.Observe();
        }

        private void Burn(int pos)
        {
            var ch = FindChar(pos);
            if (ch != null)
                Buff.Affect<Burning>(ch).Reignite(ch);

            var heap = Dungeon.Level.heaps[pos];
            if (heap != null)
                heap.Burn();
        }

        public override void Seed(int cell, int amount)
        {
            if (Cur[cell] != 0) 
                return;
            Volume += amount;
            Cur[cell] = amount;
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);
            emitter.Start(FlameParticle.Factory, 0.03f, 0);
        }

        public override string TileDesc()
        {
            return "A fire is raging here.";
        }
    }
}