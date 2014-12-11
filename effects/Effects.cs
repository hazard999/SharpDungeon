using pdsharp.noosa;

namespace sharpdungeon.effects
{
    public class Effects
    {
        public enum Type
        {
            Ripple,
            Lightning,
            Wound,
            Ray
        }

        public static Image Get(Type type)
        {
            var icon = new Image(Assets.EFFECTS);
            switch (type)
            {
                case Type.Ripple:
                    icon.Frame(icon.texture.UvRect(0, 0, 16, 16));
                    break;
                case Type.Lightning:
                    icon.Frame(icon.texture.UvRect(16, 0, 32, 8));
                    break;
                case Type.Wound:
                    icon.Frame(icon.texture.UvRect(16, 8, 32, 16));
                    break;
                case Type.Ray:
                    icon.Frame(icon.texture.UvRect(16, 16, 32, 24));
                    break;
            }
            return icon;
        }
    }
}