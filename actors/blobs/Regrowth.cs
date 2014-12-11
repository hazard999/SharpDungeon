using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.actors.blobs
{
    public class Regrowth : Blob
    {
        protected internal override void Evolve()
        {
            base.Evolve();

            if (Volume <= 0)
                return;

            var mapUpdated = false;

            for (var i = 0; i < Length; i++)
            {
                if (Off[i] <= 0)
                    continue;

                var c = Dungeon.Level.map[i];
                if (c == Terrain.EMPTY || c == Terrain.EMBERS || c == Terrain.EMPTY_DECO)
                {
                    Level.Set(i, Cur[i] > 9 ? Terrain.HIGH_GRASS : Terrain.GRASS);
                    mapUpdated = true;

                }
                else if (c == Terrain.GRASS && Cur[i] > 9)
                {
                    Level.Set(i, Terrain.HIGH_GRASS);
                    mapUpdated = true;

                }

                var ch = FindChar(i);
                if (ch != null)
                    Buff.Prolong<Roots>(ch, Tick);
            }

            if (!mapUpdated) 
                return;

            GameScene.UpdateMap();
            Dungeon.Observe();
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);

            emitter.Start(LeafParticle.LevelSpecific, 0.2f, 0);
        }
    }

}