using pdsharp.noosa;

namespace sharpdungeon
{
    public class Chrome
    {
        public enum Type
        {
            TOAST,
            TOAST_TR,
            WINDOW,
            BUTTON,
            TAG,
            GEM,
            SCROLL,
            TAB_SET,
            TAB_SELECTED,
            TAB_UNSELECTED
        }

        public static NinePatch Get(Type type)
        {
            switch (type)
            {
            case Type.WINDOW:
                return new NinePatch(Assets.CHROME, 0, 0, 22, 22, 7);
            case Type.TOAST:
                return new NinePatch(Assets.CHROME, 22, 0, 18, 18, 5);
            case Type.TOAST_TR:
                return new NinePatch(Assets.CHROME, 40, 0, 18, 18, 5);
            case Type.BUTTON:
                return new NinePatch(Assets.CHROME, 58, 0, 4, 4, 1);
            case Type.TAG:
                return new NinePatch(Assets.CHROME, 22, 18, 16, 14, 3);
            case Type.GEM:
                return new NinePatch(Assets.CHROME, 0, 32, 32, 32, 13);
            case Type.SCROLL:
                return new NinePatch(Assets.CHROME, 32, 32, 32, 32, 5, 11, 5, 11);
            case Type.TAB_SET:
                return new NinePatch(Assets.CHROME, 64, 0, 22, 22, 7, 7, 7, 7);
            case Type.TAB_SELECTED:
                return new NinePatch(Assets.CHROME, 64, 22, 10, 14, 4, 7, 4, 6);
            case Type.TAB_UNSELECTED:
                return new NinePatch(Assets.CHROME, 74, 22, 10, 14, 4, 7, 4, 6);
            default:
                return null;
            }
        }
    }
}