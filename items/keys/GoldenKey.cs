using sharpdungeon.sprites;

namespace sharpdungeon.items.keys
{
    public class GoldenKey : Key
    {
        public GoldenKey()
        {
            name = "golden key";
            image = ItemSpriteSheet.GOLDEN_KEY;
        }
        public override string Info()
        {
            return "The notches on this golden key are tiny and intricate. " + "Maybe it can open some chest lock?";
        }
    }
}