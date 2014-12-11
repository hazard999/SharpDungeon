using pdsharp.noosa.particles;
using sharpdungeon.actors.blobs;

namespace sharpdungeon.effects
{
    public class BlobEmitter : Emitter
    {
        private const int WIDTH = Blob.Width;
        private const int LENGTH = Blob.Length;

        private readonly Blob _blob;

        public BlobEmitter(Blob blob)
        {
            _blob = blob;
            blob.Use(this);
        }

        protected override void Emit(int index)
        {
            if (_blob.Volume <= 0)
                return;

            var map = _blob.Cur;
            const int size = DungeonTilemap.Size;

            for (var i = 0; i < LENGTH; i++)
            {
                if (map[i] <= 0 || !Dungeon.Visible[i]) 
                    continue;

                var x1 = ((i % WIDTH) + pdsharp.utils.Random.Float()) * size;
                var y1 = ((i / WIDTH) + pdsharp.utils.Random.Float()) * size;
                factory.Emit(this, index, x1, y1);
            }
        }
    }

}