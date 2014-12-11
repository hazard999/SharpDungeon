using System;
using pdsharp.noosa;
using sharpdungeon.actors.hero;

namespace sharpdungeon.ui
{
    public enum Icons
    {
        SKULL,
        BUSY,
        COMPASS,
        PREFS,
        WARNING,
        TARGET,
        WATA,
        WARRIOR,
        MAGE,
        ROGUE,
        HUNTRESS,
        CLOSE,
        DEPTH,
        SLEEP,
        ALERT,
        SUPPORT,
        SUPPORTED,
        BACKPACK,
        SEED_POUCH,
        SCROLL_HOLDER,
        WAND_HOLSTER,
        CHECKED,
        UNCHECKED,
        EXIT,
        CHALLENGE_OFF,
        CHALLENGE_ON
    }

    public static class IconsExtensions
    {
        public static Image Get(this Icons type)
        {
            var icon = new Image(Assets.ICONS);
            switch (type)
            {
                case Icons.SKULL:
                    icon.Frame(icon.texture.UvRect(0, 0, 8, 8));
                    break;
                case Icons.BUSY:
                    icon.Frame(icon.texture.UvRect(8, 0, 16, 8));
                    break;
                case Icons.COMPASS:
                    icon.Frame(icon.texture.UvRect(0, 8, 7, 13));
                    break;
                case Icons.PREFS:
                    icon.Frame(icon.texture.UvRect(30, 0, 46, 16));
                    break;
                case Icons.WARNING:
                    icon.Frame(icon.texture.UvRect(46, 0, 58, 12));
                    break;
                case Icons.TARGET:
                    icon.Frame(icon.texture.UvRect(0, 13, 16, 29));
                    break;
                case Icons.WATA:
                    icon.Frame(icon.texture.UvRect(30, 16, 45, 26));
                    break;
                case Icons.WARRIOR:
                    icon.Frame(icon.texture.UvRect(0, 29, 16, 45));
                    break;
                case Icons.MAGE:
                    icon.Frame(icon.texture.UvRect(16, 29, 32, 45));
                    break;
                case Icons.ROGUE:
                    icon.Frame(icon.texture.UvRect(32, 29, 48, 45));
                    break;
                case Icons.HUNTRESS:
                    icon.Frame(icon.texture.UvRect(48, 29, 64, 45));
                    break;
                case Icons.CLOSE:
                    icon.Frame(icon.texture.UvRect(0, 45, 13, 58));
                    break;
                case Icons.DEPTH:
                    icon.Frame(icon.texture.UvRect(45, 12, 54, 20));
                    break;
                case Icons.SLEEP:
                    icon.Frame(icon.texture.UvRect(13, 45, 22, 53));
                    break;
                case Icons.ALERT:
                    icon.Frame(icon.texture.UvRect(22, 45, 30, 53));
                    break;
                case Icons.SUPPORT:
                    icon.Frame(icon.texture.UvRect(30, 45, 46, 61));
                    break;
                case Icons.SUPPORTED:
                    icon.Frame(icon.texture.UvRect(46, 45, 62, 61));
                    break;
                case Icons.BACKPACK:
                    icon.Frame(icon.texture.UvRect(58, 0, 68, 10));
                    break;
                case Icons.SCROLL_HOLDER:
                    icon.Frame(icon.texture.UvRect(68, 0, 78, 10));
                    break;
                case Icons.SEED_POUCH:
                    icon.Frame(icon.texture.UvRect(78, 0, 88, 10));
                    break;
                case Icons.WAND_HOLSTER:
                    icon.Frame(icon.texture.UvRect(88, 0, 98, 10));
                    break;
                case Icons.CHECKED:
                    icon.Frame(icon.texture.UvRect(54, 12, 66, 24));
                    break;
                case Icons.UNCHECKED:
                    icon.Frame(icon.texture.UvRect(66, 12, 78, 24));
                    break;
                case Icons.EXIT:
                    icon.Frame(icon.texture.UvRect(98, 0, 114, 16));
                    break;
                case Icons.CHALLENGE_OFF:
                    icon.Frame(icon.texture.UvRect(78, 16, 102, 40));
                    break;
                case Icons.CHALLENGE_ON:
                    icon.Frame(icon.texture.UvRect(102, 16, 126, 40));
                    break;
            }
            return icon;
        }

        public static Image Get(this HeroClass cl)
        {
            switch (cl.Ordinal())
            {
                case HeroClassType.Warrior:
                    return Icons.WARRIOR.Get();
                case HeroClassType.Mage:
                    return Icons.MAGE.Get();
                case HeroClassType.Rogue:
                    return Icons.ROGUE.Get();
                case HeroClassType.Huntress:
                    return Icons.HUNTRESS.Get();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}