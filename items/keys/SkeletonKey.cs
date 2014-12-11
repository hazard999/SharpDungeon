using sharpdungeon.sprites;

namespace sharpdungeon.items.keys
{
    public class SkeletonKey : Key
    {
        public SkeletonKey()
        {
            name = "skeleton key";
            image = ItemSpriteSheet.SKELETON_KEY;
        }

        public override string Info()
        {
            return "This key looks serious: its head is shaped like a skull. " + "Probably it can open some serious door.";
        }
    }
}