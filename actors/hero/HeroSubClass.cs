using pdsharp.utils;
using System;

namespace sharpdungeon.actors.hero
{
    public enum HeroSubClassType
    {
        NONE,
        GLADIATOR,
        BERSERKER,
        WARLOCK,
        BATTLEMAGE,
        ASSASSIN,
        FREERUNNER,
        SNIPER,
        WARDEN
    }
    public class HeroSubClass
    {
        public static HeroSubClass NONE = new HeroSubClass(HeroSubClassType.NONE, null, null);

        public static HeroSubClass GLADIATOR = new HeroSubClass(HeroSubClassType.GLADIATOR, "gladiator", "A successful attack with a melee weapon allows the _Gladiator_ to start a combo, " + "in which every next successful hit inflicts more damage.");
        public static HeroSubClass BERSERKER = new HeroSubClass(HeroSubClassType.BERSERKER, "berserker", "When severely wounded, the _Berserker_ enters a state of wild fury " + "significantly increasing his damage output.");

        public static HeroSubClass WARLOCK = new HeroSubClass(HeroSubClassType.WARLOCK, "warlock", "After killing an enemy the _Warlock_ consumes its soul. " + "It heals his wounds and satisfies his hunger.");
        public static HeroSubClass BATTLEMAGE = new HeroSubClass(HeroSubClassType.BATTLEMAGE, "battlemage", "When fighting with a wand in his hands, the _Battlemage_ inflicts additional damage depending " + "on the current number of charges. Every successful hit restores 1 charge to this wand.");

        public static HeroSubClass ASSASSIN = new HeroSubClass(HeroSubClassType.ASSASSIN, "assassin", "When performing a surprise attack, the _Assassin_ inflicts additional damage to his target.");

        public static HeroSubClass FREERUNNER = new HeroSubClass(HeroSubClassType.FREERUNNER, "freerunner", "The _Freerunner_ can move almost twice faster, than most of the monsters. When he " + "is running, the Freerunner is much harder to hit. For that he must be unencumbered and not starving.");

        public static HeroSubClass SNIPER = new HeroSubClass(HeroSubClassType.SNIPER, "sniper", "_Snipers_ are able to detect weak points in an enemy's armor, " + "effectively ignoring it when using a missile weapon.");

        public static HeroSubClass WARDEN = new HeroSubClass(HeroSubClassType.WARDEN, "warden", "Having a strong connection with forces of nature gives _Wardens_ an ability to gather dewdrops and " + "seeds from plants. Also trampling a high grass grants them a temporary armor buff.");

        private readonly HeroSubClassType _subClassType;
        private readonly String _title;

        private readonly String _desc;


        private HeroSubClass(HeroSubClassType subClassType, string title, string desc)
        {
            _subClassType = subClassType;
            _title = title;
            _desc = desc;
        }

        private const String Subclass = "subClass";

        public static HeroSubClass RestoreInBundle(Bundle bundle)
        {
            var value = bundle.GetString(Subclass);
            try
            {
                switch (value)
                {
                    case "NONE":
                        return NONE;
                    case "GLADIATOR":
                        return GLADIATOR;
                    case "BERSERKER":
                        return BERSERKER;
                    case "WARLOCK":
                        return WARLOCK;
                    case "BATTLEMAGE":
                        return BATTLEMAGE;
                    case "ASSASSIN":
                        return ASSASSIN;
                    case "FREERUNNER":
                        return FREERUNNER;
                    case "SNIPER":
                        return SNIPER;
                    case "WARDEN":
                        return WARDEN;
                }
            }
            catch (Exception)
            {
                return NONE;
            }

            return NONE;
        }

        public string Title
        {
            get { return _title; }
        }

        public string Desc
        {
            get { return _desc; }
        }

        public HeroSubClassType SubClassType
        {
            get { return _subClassType; }
        }

        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put(Subclass, ToString());
        }

    }
}