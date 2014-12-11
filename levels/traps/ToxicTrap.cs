using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.scenes;

namespace sharpdungeon.levels.traps
{
    public class ToxicTrap
    {
        // 0x40CC55
        public static void Trigger(int pos, Character ch)
        {
            GameScene.Add(Blob.Seed(pos, 300 + 20 * Dungeon.Depth, typeof(ToxicGas)));
        }
    }
}