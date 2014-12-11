using System;
using System.Collections.Generic;
using System.Linq;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.armor;
using sharpdungeon.items.bags;
using sharpdungeon.items.food;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.plants;

namespace sharpdungeon.items
{
    public class Generator
    {
        public class Category
        {
            public static Category WEAPON = new Category(15, typeof(Weapon));

            public static Category ARMOR = new Category(10, typeof(Armor));

            public static Category POTION = new Category(50, typeof(Potion));

            public static Category SCROLL = new Category(40, typeof(Scroll));

            public static Category WAND = new Category(4, typeof(Wand));

            public static Category RING = new Category(2, typeof(Ring));

            public static Category SEED = new Category(5, typeof(Plant.Seed));

            public static Category FOOD = new Category(0, typeof(Food));

            public static Category GOLD = new Category(50, typeof(Gold));

            public float[] probs;

            public float prob;

            public Type superClass;

            public Type[] classes;

            private Category(float prob, Type superClass)
            {
                this.prob = prob;
                this.superClass = superClass;
            }

            public static IEnumerable<Category> Values()
            {
                yield return WEAPON;
                yield return ARMOR;
                yield return POTION;
                yield return SCROLL;
                yield return WAND;
                yield return RING;
                yield return SEED;
                yield return FOOD;
                yield return GOLD;
            }

            public static int Order(Item item)
            {
                var i = 0;
                foreach (var value in Values())
                {
                    if (value.superClass.IsInstanceOfType(item))
                        return i;

                    i++;
                }

                return item is Bag ? int.MaxValue : int.MaxValue - 1;
            }
        }

        private static readonly Dictionary<Category, float> CategoryProbs = new Dictionary<Category, float>();

        static Generator()
        {
            Category.GOLD.classes = new[] { typeof(Gold) };
            Category.GOLD.probs = new float[] { 1 };

            Category.SCROLL.classes = new[] { typeof(ScrollOfIdentify), typeof(ScrollOfTeleportation), typeof(ScrollOfRemoveCurse), typeof(ScrollOfUpgrade), typeof(ScrollOfRecharging), typeof(ScrollOfMagicMapping), typeof(ScrollOfChallenge), typeof(ScrollOfTerror), typeof(ScrollOfLullaby), typeof(ScrollOfWeaponUpgrade), typeof(ScrollOfPsionicBlast), typeof(ScrollOfMirrorImage) };
            Category.SCROLL.probs = new float[] { 30, 10, 15, 0, 10, 15, 12, 8, 8, 0, 4, 6 };

            Category.POTION.classes = new[] { typeof(PotionOfHealing), typeof(PotionOfExperience), typeof(PotionOfToxicGas), typeof(PotionOfParalyticGas), typeof(PotionOfLiquidFlame), typeof(PotionOfLevitation), typeof(PotionOfStrength), typeof(PotionOfMindVision), typeof(PotionOfPurity), typeof(PotionOfInvisibility), typeof(PotionOfMight), typeof(PotionOfFrost) };
            Category.POTION.probs = new float[] { 45, 4, 15, 10, 15, 10, 0, 20, 12, 10, 0, 10 };

            Category.WAND.classes = new[] { typeof(WandOfTeleportation), typeof(WandOfSlowness), typeof(WandOfFirebolt), typeof(WandOfRegrowth), typeof(WandOfPoison), typeof(WandOfBlink), typeof(WandOfLightning), typeof(WandOfAmok), typeof(WandOfTelekinesis), typeof(WandOfFlock), typeof(WandOfMagicMissile), typeof(WandOfDisintegration), typeof(WandOfAvalanche) };
            Category.WAND.probs = new float[] { 10, 10, 15, 6, 10, 11, 15, 10, 6, 10, 0, 5, 5 };

            Category.WEAPON.classes = new[] { typeof(Dagger), typeof(Knuckles), typeof(Quarterstaff), typeof(Spear), typeof(Mace), typeof(Sword), typeof(Longsword), typeof(BattleAxe), typeof(WarHammer), typeof(Glaive), typeof(ShortSword), typeof(Dart), typeof(Javelin), typeof(IncendiaryDart), typeof(CurareDart), typeof(Shuriken), typeof(Boomerang), typeof(Tamahawk) };
            Category.WEAPON.probs = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1 };

            Category.ARMOR.classes = new[] { typeof(ClothArmor), typeof(LeatherArmor), typeof(MailArmor), typeof(ScaleArmor), typeof(PlateArmor) };
            Category.ARMOR.probs = new float[] { 1, 1, 1, 1, 1 };

            Category.FOOD.classes = new[] { typeof(Food), typeof(Pasty), typeof(MysteryMeat) };
            Category.FOOD.probs = new float[] { 4, 1, 0 };

            Category.RING.classes = new[] { typeof(RingOfMending), typeof(RingOfDetection), typeof(RingOfShadows), typeof(RingOfPower), typeof(RingOfHerbalism), typeof(RingOfAccuracy), typeof(RingOfEvasion), typeof(RingOfSatiety), typeof(RingOfHaste), typeof(RingOfElements), typeof(RingOfHaggler), typeof(RingOfThorns) };
            Category.RING.probs = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 };

            Category.SEED.classes = new[] { typeof(Firebloom.Seed), typeof(Icecap.Seed), typeof(Sorrowmoss.Seed), typeof(Dreamweed.Seed), typeof(Sungrass.Seed), typeof(Earthroot.Seed), typeof(Fadeleaf.Seed), typeof(Wandmaker.Rotberry.Seed) };
            Category.SEED.probs = new float[] { 1, 1, 1, 1, 1, 1, 1, 0 };
        }

        public static void Reset()
        {
            foreach (var cat in Category.Values().Where(cat => !CategoryProbs.ContainsKey(cat)))
                CategoryProbs.Add(cat, cat.prob);
        }

        public static Item Random()
        {
            return Random(pdsharp.utils.Random.Chances(CategoryProbs));
        }

        public static Item Random(Category cat)
        {
            try
            {
                if (!CategoryProbs.ContainsKey(cat))
                    CategoryProbs.Add(cat, cat.prob / 2);

                if (cat == Category.ARMOR)
                    return RandomArmor();
                if (cat == Category.WEAPON)
                    return RandomWeapon();

                return ((Item)Activator.CreateInstance(cat.classes[pdsharp.utils.Random.Chances(cat.probs)])).Random();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Item Random(Type cl)
        {
            try
            {
                var item = (Item)Activator.CreateInstance(cl);
                return item.Random();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Armor RandomArmor()
        {
            var curStr = Hero.StartingStr + Dungeon.PotionOfStrength;

            var cat = Category.ARMOR;

            var a1 = (Armor)Activator.CreateInstance(cat.classes[pdsharp.utils.Random.Chances(cat.probs)]);
            var a2 = (Armor)Activator.CreateInstance(cat.classes[pdsharp.utils.Random.Chances(cat.probs)]);

            a1.Random();
            a2.Random();

            return Math.Abs(curStr - a1.Str) < Math.Abs(curStr - a2.Str) ? a1 : a2;
        }

        public static Weapon RandomWeapon()
        {
            var curStr = Hero.StartingStr + Dungeon.PotionOfStrength;

            var cat = Category.WEAPON;

            var w1 = (Weapon)Activator.CreateInstance(cat.classes[pdsharp.utils.Random.Chances(cat.probs)]);
            var w2 = (Weapon)Activator.CreateInstance(cat.classes[pdsharp.utils.Random.Chances(cat.probs)]);

            w1.Random();
            w2.Random();

            return Math.Abs(curStr - w1.Str) < Math.Abs(curStr - w2.Str) ? w1 : w2;
        }
    }
}