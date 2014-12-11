using pdsharp.utils;

namespace sharpdungeon.items.keys
{
    public class Key : Item
    {
        public const float TIME_TO_UNLOCK = 1f;

        public int depth;

        public Key()
        {
            depth = Dungeon.Depth;
            Stackable = false;
        }

        private const string DEPTH = "depth";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(DEPTH, depth);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            depth = bundle.GetInt(DEPTH);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override string Status()
        {
            return depth + "*";
        }
    }
}