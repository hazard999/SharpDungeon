using pdsharp.noosa;

namespace sharpdungeon.effects
{
    public class BannerSprites
    {
        public enum Type
        {
            PixelDungeon,
            BossSlain,
            GameOver,
            SelectYourHero
        }

        public static Image Get(Type type)
        {
            var icon = new Image(Assets.BANNERS);
            switch (type)
            {
                case Type.PixelDungeon:
                    icon.Frame(icon.texture.UvRect(0, 0, 128, 70));
                    break;
                case Type.BossSlain:
                    icon.Frame(icon.texture.UvRect(0, 70, 128, 105));
                    break;
                case Type.GameOver:
                    icon.Frame(icon.texture.UvRect(0, 105, 128, 140));
                    break;
                case Type.SelectYourHero:
                    icon.Frame(icon.texture.UvRect(0, 140, 128, 161));
                    break;
            }
            return icon;
        }
    }
}