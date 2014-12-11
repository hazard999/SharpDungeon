using System;
using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.armor;
using sharpdungeon.items.food;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.items.weapon.missiles;

namespace sharpdungeon.actors.hero
{
    public class HeroClass
    {
        public static HeroClass Warrior = new HeroClass("warrior");
        public static HeroClass Mage = new HeroClass("mage");
        public static HeroClass Rogue = new HeroClass("rogue");
        public static HeroClass Huntress = new HeroClass("huntress");

        private readonly String _title;

        public HeroClass(String title)
        {
            _title = title;
        }

        public static String[] WarPerks = { "Warriors start with 11 points of Strength.", "Warriors start with a unique short sword. This sword can be later \"reforged\" to Upgrade another melee weapon.", "Warriors are less proficient with missile weapons.", "Any piece of food restores some health when eaten.", "Potions of Strength are identified from the beginning." };
        public static String[] MagPerks = { "Mages start with a unique Wand of Magic Factory. This wand can be later \"disenchanted\" to Upgrade another wand.", "Mages recharge their wands faster.", "When eaten, any piece of food restores 1 charge for All wands in the inventory.", "Mages can use wands as a melee weapon.", "Scrolls of Identify are identified from the beginning." };
        public static String[] RogPerks = { "Rogues start with a Ring of Shadows+1.", "Rogues identify a type of a ring on equipping it.", "Rogues are proficient with light armor, dodging better while wearing one.", "Rogues are proficient in detecting hidden doors and traps.", "Rogues can go without food longer.", "Scrolls of Magic Mapping are identified from the beginning." };
        public static String[] HunPerks = { "Huntresses start with 15 points of Health.", "Huntresses start with a unique upgradeable boomerang.", "Huntresses are proficient with missile weapons and Get damage bonus for excessive strength when using them.", "Huntresses gain more health from dewdrops.", "Huntresses sense neighbouring monsters even if they are hidden behind obstacles." };

        private static void InitCommon(Hero hero)
        {
            (hero.Belongings.Armor = new ClothArmor()).Identify();
            new Food().Identify().Collect();
        }

        private static void InitWarrior(Hero hero)
        {
            hero.STR = hero.STR + 1;

            (hero.Belongings.Weapon = new ShortSword()).Identify();
            new Dart(8).Identify().Collect();

            Dungeon.Quickslot = new Dart();

            new PotionOfStrength().SetKnown();
        }

        private static void InitMage(Hero hero)
        {
            (hero.Belongings.Weapon = new Knuckles()).Identify();

            WandOfMagicMissile wand = new WandOfMagicMissile();
            wand.Identify().Collect();

            Dungeon.Quickslot = wand;

            new ScrollOfIdentify().SetKnown();
        }

        private static void InitRogue(Hero hero)
        {
            (hero.Belongings.Weapon = new Dagger()).Identify();
            (hero.Belongings.Ring1 = new RingOfShadows()).Upgrade().Identify();
            new Dart(8).Identify().Collect();

            hero.Belongings.Ring1.Activate(hero);

            Dungeon.Quickslot = new Dart();

            new ScrollOfMagicMapping().SetKnown();
        }

        private static void InitHuntress(Hero hero)
        {
            hero.HP = (hero.HT -= 5);

            (hero.Belongings.Weapon = new Dagger()).Identify();
            var boomerang = new Boomerang();
            boomerang.Identify().Collect();

            Dungeon.Quickslot = boomerang;
        }

        private const String Class = "class";

        public static HeroClass ReStoreInBundle(Bundle bundle)
        {
            var value = bundle.GetString(Class);

            if (string.IsNullOrEmpty(value))
                return Rogue;

            switch (value)
            {
                case "Warrior":
                    return Warrior;
                case "Mage":
                    return Mage;
                case "Rogue":
                    return Rogue;
                case "Huntress":
                    return Huntress;
            }

            return Rogue;
        }

        public HeroClassType Ordinal()
        {
            if (this == Warrior)
                return HeroClassType.Warrior;

            if (this == Mage)
                return HeroClassType.Mage;

            if (this == Rogue)
                return HeroClassType.Rogue;

            if (this == Huntress)
                return HeroClassType.Huntress;

            return HeroClassType.Rogue;
        }

        public void InitHero(Hero hero)
        {
            hero.heroClass = this;

            InitCommon(hero);

            if (this == Warrior)
                InitWarrior(hero);

            if (this == Mage)
                InitMage(hero);

            if (this == Rogue)
                InitRogue(hero);

            if (this == Huntress)
                InitHuntress(hero);

            if (Badge.IsUnlocked(MasteryBadge()))
                new TomeOfMastery().Collect();

            hero.UpdateAwareness();
        }

        public Badge MasteryBadge()
        {
            if (this == Warrior)
                return Badge.MASTERY_WARRIOR;

            if (this == Mage)
                return Badge.MASTERY_MAGE;

            if (this == Rogue)
                return Badge.MASTERY_ROGUE;

            if (this == Huntress)
                return Badge.MASTERY_HUNTRESS;

            return null;
        }

        public string Title()
        {
            return _title;
        }

        public string Spritesheet()
        {
            if (this == Warrior)
                return Assets.WARRIOR;

            if (this == Mage)
                return Assets.MAGE;

            if (this == Rogue)
                return Assets.ROGUE;

            if (this == Huntress)
                return Assets.HUNTRESS;

            return null;
        }

        public string[] Perks()
        {
            if (this == Warrior)
                return WarPerks;

            if (this == Mage)
                return MagPerks;

            if (this == Rogue)
                return RogPerks;

            if (this == Huntress)
                return HunPerks;

            return null;
        }
        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put(Class, ToString());
        }

        public static IList<HeroClass> Values()
        {
            var result = new List<HeroClass>();

            result.Add(Warrior);
            result.Add(Mage);
            result.Add(Rogue);
            result.Add(Huntress);

            return result;
        }
    }

    public enum HeroClassType
    {
        Warrior,
        Mage,
        Rogue,
        Huntress
    }
}