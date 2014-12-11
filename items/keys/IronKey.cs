using sharpdungeon.items.bags;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.keys
{
    public class IronKey : Key
    {
        private const string TxtFromDepth = "iron key from depth {0}";

        public static int CurDepthQuantity = 0;

        public IronKey()
        {
            name = "iron key";
            image = ItemSpriteSheet.IRON_KEY;
        }

        public override bool Collect(Bag bag)
        {
            var result = base.Collect(bag);
            
            if (result && depth == Dungeon.Depth && Dungeon.Hero != null)
                Dungeon.Hero.Belongings.CountIronKeys();

            return result;
        }

        protected override void OnDetach()
        {
            if (depth == Dungeon.Depth)
                Dungeon.Hero.Belongings.CountIronKeys();
        }

        public override string ToString()
        {
            return Utils.Format(TxtFromDepth, depth);
        }

        public override string Info()
        {
            return "The notches on this ancient iron key are well worn; its leather lanyard " + "is battered by age. What door might it open?";
        }
    }
}