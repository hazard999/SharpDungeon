using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.scenes;

namespace sharpdungeon.levels.traps
{
    public class ParalyticTrap
    {
        // 0xCCCC55
        public static void Trigger(int pos, Character ch)
        {
            GameScene.Add(Blob.Seed(pos, 80 + 5 * Dungeon.Depth, typeof(ParalyticGas)));
        }
    }
}