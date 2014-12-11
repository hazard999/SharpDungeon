using pdsharp.noosa.audio;
using sharpdungeon.scenes;

namespace sharpdungeon.levels.features
{
    public class Door
    {
        public static void Enter(int pos)
        {
            Level.Set(pos, Terrain.OPEN_DOOR);
            GameScene.UpdateMap(pos);
            Dungeon.Observe();

            if (Dungeon.Visible[pos])
                Sample.Instance.Play(Assets.SND_OPEN);
        }

        public static void Leave(int pos)
        {
            if (Dungeon.Level.heaps[pos] != null) 
                return;

            Level.Set(pos, Terrain.DOOR);
            GameScene.UpdateMap(pos);
            Dungeon.Observe();
        }
    }
}