using pdsharp.noosa.particles;
using sharpdungeon.scenes;

namespace sharpdungeon.effects
{
    public class CellEmitter
    {
        public static Emitter Get(int cell)
        {
            var p = DungeonTilemap.TileToWorld(cell);

            var emitter = GameScene.Emitter();
            emitter.Pos(p.X, p.Y, DungeonTilemap.Size, DungeonTilemap.Size);

            return emitter;
        }

        public static Emitter Center(int cell)
        {
            var p = DungeonTilemap.TileToWorld(cell);

            var emitter = GameScene.Emitter();
            emitter.Pos(p.X + DungeonTilemap.Size / 2, p.Y + DungeonTilemap.Size / 2);

            return emitter;
        }

        public static Emitter Bottom(int cell)
        {
            var p = DungeonTilemap.TileToWorld(cell);

            var emitter = GameScene.Emitter();
            emitter.Pos(p.X, p.Y + DungeonTilemap.Size, DungeonTilemap.Size, 0);

            return emitter;
        }
    }
}