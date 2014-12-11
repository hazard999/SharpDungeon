using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.items.bags;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon
{
    public class Badge
    {
        public static Badge MONSTERS_SLAIN_1 = new Badge("10 enemies slain", 0);

        public static Badge MONSTERS_SLAIN_2 = new Badge("50 enemies slain", 1);
        public static Badge MONSTERS_SLAIN_3 = new Badge("150 enemies slain", 2);
        public static Badge MONSTERS_SLAIN_4 = new Badge("250 enemies slain", 3);
        public static Badge GOLD_COLLECTED_1 = new Badge("100 gold collected", 4);
        public static Badge GOLD_COLLECTED_2 = new Badge("500 gold collected", 5);
        public static Badge GOLD_COLLECTED_3 = new Badge("2500 gold collected", 6);
        public static Badge GOLD_COLLECTED_4 = new Badge("7500 gold collected", 7);
        public static Badge LEVEL_REACHED_1 = new Badge("Level 6 reached", 8);
        public static Badge LEVEL_REACHED_2 = new Badge("Level 12 reached", 9);
        public static Badge LEVEL_REACHED_3 = new Badge("Level 18 reached", 10);
        public static Badge LEVEL_REACHED_4 = new Badge("Level 24 reached", 11);
        public static Badge ALL_POTIONS_IDENTIFIED = new Badge("All potions identified", 16);
        public static Badge ALL_SCROLLS_IDENTIFIED = new Badge("All scrolls identified", 17);
        public static Badge ALL_RINGS_IDENTIFIED = new Badge("All rings identified", 18);
        public static Badge ALL_WANDS_IDENTIFIED = new Badge("All wands identified", 19);

        public static Badge ALL_ITEMS_IDENTIFIED = new Badge("All potions, scrolls, rings & wands identified", 35, true);

        public static Badge BAG_BOUGHT_SEED_POUCH;
        public static Badge BAG_BOUGHT_SCROLL_HOLDER;
        public static Badge BAG_BOUGHT_WAND_HOLSTER;

        public static Badge ALL_BAGS_BOUGHT = new Badge("All bags bought", 23);

        public static Badge DEATH_FROM_FIRE = new Badge("Death from fire", 24);

        public static Badge DEATH_FROM_POISON = new Badge("Death from poison", 25);

        public static Badge DEATH_FROM_GAS = new Badge("Death from toxic gas", 26);

        public static Badge DEATH_FROM_HUNGER = new Badge("Death from hunger", 27);

        public static Badge DEATH_FROM_GLYPH = new Badge("Death from a glyph", 57);

        public static Badge DEATH_FROM_FALLING = new Badge("Death from falling down", 59);

        public static Badge YASD = new Badge("Death from fire, poison, toxic gas & hunger", 34, true);
        public static Badge BOSS_SLAIN_1_WARRIOR;
        public static Badge BOSS_SLAIN_1_MAGE;
        public static Badge BOSS_SLAIN_1_ROGUE;
        public static Badge BOSS_SLAIN_1_HUNTRESS;

        public static Badge BOSS_SLAIN_1 = new Badge("1st boss slain", 12);

        public static Badge BOSS_SLAIN_2 = new Badge("2nd boss slain", 13);

        public static Badge BOSS_SLAIN_3 = new Badge("3rd boss slain", 14);

        public static Badge BOSS_SLAIN_4 = new Badge("4th boss slain", 15);

        public static Badge BOSS_SLAIN_1_ALL_CLASSES = new Badge("1st boss slain by Warrior, Mage, Rogue & Huntress", 32, true);
        public static Badge BOSS_SLAIN_3_GLADIATOR;
        public static Badge BOSS_SLAIN_3_BERSERKER;
        public static Badge BOSS_SLAIN_3_WARLOCK;
        public static Badge BOSS_SLAIN_3_BATTLEMAGE;
        public static Badge BOSS_SLAIN_3_FREERUNNER;
        public static Badge BOSS_SLAIN_3_ASSASSIN;
        public static Badge BOSS_SLAIN_3_SNIPER;
        public static Badge BOSS_SLAIN_3_WARDEN;

        public static Badge BOSS_SLAIN_3_ALL_SUBCLASSES = new Badge("3rd boss slain by Gladiator, Berserker, Warlock, Battlemage, Freerunner, Assassin, Sniper & Warden", 33, true);

        public static Badge RING_OF_HAGGLER = new Badge("Ring of Haggler obtained", 20);

        public static Badge RING_OF_THORNS = new Badge("Ring of Thorns obtained", 21);

        public static Badge STRENGTH_ATTAINED_1 = new Badge("13 points of Strength attained", 40);

        public static Badge STRENGTH_ATTAINED_2 = new Badge("15 points of Strength attained", 41);

        public static Badge STRENGTH_ATTAINED_3 = new Badge("17 points of Strength attained", 42);

        public static Badge STRENGTH_ATTAINED_4 = new Badge("19 points of Strength attained", 43);

        public static Badge FOOD_EATEN_1 = new Badge("10 pieces of food eaten", 44);

        public static Badge FOOD_EATEN_2 = new Badge("20 pieces of food eaten", 45);

        public static Badge FOOD_EATEN_3 = new Badge("30 pieces of food eaten", 46);

        public static Badge FOOD_EATEN_4 = new Badge("40 pieces of food eaten", 47);
        public static Badge MASTERY_WARRIOR;
        public static Badge MASTERY_MAGE;
        public static Badge MASTERY_ROGUE;
        public static Badge MASTERY_HUNTRESS;

        public static Badge ITEM_LEVEL_1 = new Badge("Item of Level 3 acquired", 48);

        public static Badge ITEM_LEVEL_2 = new Badge("Item of Level 6 acquired", 49);

        public static Badge ITEM_LEVEL_3 = new Badge("Item of Level 9 acquired", 50);

        public static Badge ITEM_LEVEL_4 = new Badge("Item of Level 12 acquired", 51);
        public static Badge RARE_ALBINO;
        public static Badge RARE_BANDIT;
        public static Badge RARE_SHIELDED;
        public static Badge RARE_SENIOR;
        public static Badge RARE_ACIDIC;

        public static Badge RARE = new Badge("All rare monsters slain", 37, true);
        public static Badge VICTORY_WARRIOR;
        public static Badge VICTORY_MAGE;
        public static Badge VICTORY_ROGUE;
        public static Badge VICTORY_HUNTRESS;

        public static Badge VICTORY = new Badge("Amulet of Yendor obtained", 22);

        public static Badge VICTORY_ALL_CLASSES = new Badge("Amulet of Yendor obtained by Warrior, Mage, Rogue & Huntress", 36, true);

        public static Badge MASTERY_COMBO = new Badge("7-hit combo", 56);

        public static Badge POTIONS_COOKED_1 = new Badge("3 potions cooked", 52);

        public static Badge POTIONS_COOKED_2 = new Badge("6 potions cooked", 53);

        public static Badge POTIONS_COOKED_3 = new Badge("9 potions cooked", 54);

        public static Badge POTIONS_COOKED_4 = new Badge("12 potions cooked", 55);

        public static Badge NO_MONSTERS_SLAIN = new Badge("Level completed without killing any monsters", 28);

        public static Badge GRIM_WEAPON = new Badge("Monster killed by a Grim weapon", 29);

        public static Badge PIRANHAS = new Badge("6 piranhas killed", 30);

        public static Badge NIGHT_HUNTER = new Badge("15 monsters killed at nighttime", 58);

        public static Badge GAMES_PLAYED_1 = new Badge("10 games played", 60, true);

        public static Badge GAMES_PLAYED_2 = new Badge("100 games played", 61, true);

        public static Badge GAMES_PLAYED_3 = new Badge("500 games played", 62, true);

        public static Badge GAMES_PLAYED_4 = new Badge("2000 games played", 63, true);

        public static Badge HAPPY_END = new Badge("Happy end", 38);

        public static Badge CHAMPION = new Badge("Challenge won", 39, true);

        public static Badge SUPPORTER = new Badge("Thanks for your support!", 31, true);


        public bool Meta;

        public string Description;

        public int Image;


        private Badge(string description, int image, bool meta = false)
        {
            Description = description;
            Image = image;
            Meta = meta;
        }

        private Badge()
            : this("", -1)
        {
        }

        private static List<Badge> _global;
        private static List<Badge> _local = new List<Badge>();

        private static bool _saveNeeded;

        public static void Reset()
        {
            _local.Clear();
            LoadGlobal();
        }

        private const string BadgesFile = "badges.dat";
        private const string Badges = "badges";

        private static List<Badge> Restore(Bundle bundle)
        {
            var badges = new List<Badge>();

            var names = bundle.GetStringArray(Badges);
            foreach (var name in names)
            {
                try
                {
                    //badges.Add(new Badge(

                    //    Enum.Parse = new Badge(typeof = new Badge(Badge), names[i])

                    //    );
                }
                catch (Exception)
                {
                }
            }

            return badges;
        }

        private static void Store(Bundle bundle, IEnumerable<Badge> badges)
        {
            var count = 0;
            var names = new string[badges.Count()];

            foreach (var badge in badges)
                names[count++] = badge.ToString();

            bundle.Put(Badges, names);
        }

        public static void LoadLocal(Bundle bundle)
        {
            _local = Restore(bundle);
        }

        public static void SaveLocal(Bundle bundle)
        {
            Store(bundle, _local);
        }

        public static void LoadGlobal()
        {
            if (_global != null)
                return;

            //Game.Instance.FilesDir.
            //try
            //{
            //    var input = Game.Instance.OpenFileInput(BadgesFile);
            //    var bundle = Bundle.Read(input);
            //    input.Close();

            //    _global = Restore(bundle);

            //}
            //catch (IOException)
            //{
                _global = new List<Badge>();
            //}
        }

        public static void SaveGlobal()
        {
            if (!_saveNeeded)
                return;

            var bundle = new Bundle();
            Store(bundle, _global);

            try
            {
                var output = Game.Instance.OpenFileOutput(BadgesFile, FileCreationMode.Private);
                Bundle.Write(bundle, output);
                output.Close();
                _saveNeeded = false;
            }
            catch (IOException)
            {

            }
        }

        public static void ValidateMonstersSlain()
        {
            Badge badge = null;

            if (!_local.Contains(MONSTERS_SLAIN_1) && Statistics.EnemiesSlain >= 10)
            {
                badge = MONSTERS_SLAIN_1;
                _local.Add(badge);
            }
            if (!_local.Contains(MONSTERS_SLAIN_2) && Statistics.EnemiesSlain >= 50)
            {
                badge = MONSTERS_SLAIN_2;
                _local.Add(badge);
            }
            if (!_local.Contains(MONSTERS_SLAIN_3) && Statistics.EnemiesSlain >= 150)
            {
                badge = MONSTERS_SLAIN_3;
                _local.Add(badge);
            }
            if (!_local.Contains(MONSTERS_SLAIN_4) && Statistics.EnemiesSlain >= 250)
            {
                badge = MONSTERS_SLAIN_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateGoldCollected()
        {
            Badge badge = null;

            if (!_local.Contains(GOLD_COLLECTED_1) && Statistics.GoldCollected >= 100)
            {
                badge = GOLD_COLLECTED_1;
                _local.Add(badge);
            }
            if (!_local.Contains(GOLD_COLLECTED_2) && Statistics.GoldCollected >= 500)
            {
                badge = GOLD_COLLECTED_2;
                _local.Add(badge);
            }
            if (!_local.Contains(GOLD_COLLECTED_3) && Statistics.GoldCollected >= 2500)
            {
                badge = GOLD_COLLECTED_3;
                _local.Add(badge);
            }
            if (!_local.Contains(GOLD_COLLECTED_4) && Statistics.GoldCollected >= 7500)
            {
                badge = GOLD_COLLECTED_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateLevelReached()
        {
            Badge badge = null;

            if (!_local.Contains(LEVEL_REACHED_1) && Dungeon.Hero.Lvl >= 6)
            {
                badge = LEVEL_REACHED_1;
                _local.Add(badge);
            }
            if (!_local.Contains(LEVEL_REACHED_2) && Dungeon.Hero.Lvl >= 12)
            {
                badge = LEVEL_REACHED_2;
                _local.Add(badge);
            }
            if (!_local.Contains(LEVEL_REACHED_3) && Dungeon.Hero.Lvl >= 18)
            {
                badge = LEVEL_REACHED_3;
                _local.Add(badge);
            }
            if (!_local.Contains(LEVEL_REACHED_4) && Dungeon.Hero.Lvl >= 24)
            {
                badge = LEVEL_REACHED_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateStrengthAttained()
        {
            Badge badge = null;

            if (!_local.Contains(STRENGTH_ATTAINED_1) && Dungeon.Hero.STR >= 13)
            {
                badge = STRENGTH_ATTAINED_1;
                _local.Add(badge);
            }
            if (!_local.Contains(STRENGTH_ATTAINED_2) && Dungeon.Hero.STR >= 15)
            {
                badge = STRENGTH_ATTAINED_2;
                _local.Add(badge);
            }
            if (!_local.Contains(STRENGTH_ATTAINED_3) && Dungeon.Hero.STR >= 17)
            {
                badge = STRENGTH_ATTAINED_3;
                _local.Add(badge);
            }
            if (!_local.Contains(STRENGTH_ATTAINED_4) && Dungeon.Hero.STR >= 19)
            {
                badge = STRENGTH_ATTAINED_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateFoodEaten()
        {
            Badge badge = null;

            if (!_local.Contains(FOOD_EATEN_1) && Statistics.FoodEaten >= 10)
            {
                badge = FOOD_EATEN_1;
                _local.Add(badge);
            }
            if (!_local.Contains(FOOD_EATEN_2) && Statistics.FoodEaten >= 20)
            {
                badge = FOOD_EATEN_2;
                _local.Add(badge);
            }
            if (!_local.Contains(FOOD_EATEN_3) && Statistics.FoodEaten >= 30)
            {
                badge = FOOD_EATEN_3;
                _local.Add(badge);
            }
            if (!_local.Contains(FOOD_EATEN_4) && Statistics.FoodEaten >= 40)
            {
                badge = FOOD_EATEN_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidatePotionsCooked()
        {
            Badge badge = null;

            if (!_local.Contains(POTIONS_COOKED_1) && Statistics.PotionsCooked >= 3)
            {
                badge = POTIONS_COOKED_1;
                _local.Add(badge);
            }
            if (!_local.Contains(POTIONS_COOKED_2) && Statistics.PotionsCooked >= 6)
            {
                badge = POTIONS_COOKED_2;
                _local.Add(badge);
            }
            if (!_local.Contains(POTIONS_COOKED_3) && Statistics.PotionsCooked >= 9)
            {
                badge = POTIONS_COOKED_3;
                _local.Add(badge);
            }
            if (!_local.Contains(POTIONS_COOKED_4) && Statistics.PotionsCooked >= 12)
            {
                badge = POTIONS_COOKED_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidatePiranhasKilled()
        {
            Badge badge = null;

            if (!_local.Contains(PIRANHAS) && Statistics.PiranhasKilled >= 6)
            {
                badge = PIRANHAS;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateItemLevelAquired(Item item)
        {

            // This method should be called:
            // 1) When an item is obtained  (Item.collect)
            // 2) When an item is upgraded  (ScrollOfUpgrade, ScrollOfWeaponUpgrade, ShortSword, WandOfMagicMissile)
            // 3) When an item is identified
            if (!item.levelKnown)
            {
                return;
            }

            Badge badge = null;

            if (!_local.Contains(ITEM_LEVEL_1) && item.level >= 3)
            {
                badge = ITEM_LEVEL_1;
                _local.Add(badge);
            }
            if (!_local.Contains(ITEM_LEVEL_2) && item.level >= 6)
            {
                badge = ITEM_LEVEL_2;
                _local.Add(badge);
            }
            if (!_local.Contains(ITEM_LEVEL_3) && item.level >= 9)
            {
                badge = ITEM_LEVEL_3;
                _local.Add(badge);
            }
            if (!_local.Contains(ITEM_LEVEL_4) && item.level >= 12)
            {
                badge = ITEM_LEVEL_4;
                _local.Add(badge);
            }

            DisplayBadge(badge);
        }

        public static void ValidateAllPotionsIdentified()
        {
            if (Dungeon.Hero != null && Dungeon.Hero.IsAlive && !_local.Contains(ALL_POTIONS_IDENTIFIED) && Potion.AllKnown())
            {

                Badge badge = ALL_POTIONS_IDENTIFIED;
                _local.Add(badge);
                DisplayBadge(badge);

                ValidateAllItemsIdentified();
            }
        }

        public static void ValidateAllScrollsIdentified()
        {
            if (Dungeon.Hero != null && Dungeon.Hero.IsAlive && !_local.Contains(ALL_SCROLLS_IDENTIFIED) && Scroll.AllKnown())
            {

                Badge badge = ALL_SCROLLS_IDENTIFIED;
                _local.Add(badge);
                DisplayBadge(badge);

                ValidateAllItemsIdentified();
            }
        }

        public static void ValidateAllRingsIdentified()
        {
            if (Dungeon.Hero != null && Dungeon.Hero.IsAlive && !_local.Contains(ALL_RINGS_IDENTIFIED) && Ring.AllKnown())
            {

                Badge badge = ALL_RINGS_IDENTIFIED;
                _local.Add(badge);
                DisplayBadge(badge);

                ValidateAllItemsIdentified();
            }
        }

        public static void ValidateAllWandsIdentified()
        {
            if (Dungeon.Hero != null && Dungeon.Hero.IsAlive && !_local.Contains(ALL_WANDS_IDENTIFIED) && Wand.AllKnown())
            {

                Badge badge = ALL_WANDS_IDENTIFIED;
                _local.Add(badge);
                DisplayBadge(badge);

                ValidateAllItemsIdentified();
            }
        }

        public static void ValidateAllBagsBought(Item bag)
        {

            Badge badge = null;
            if (bag is SeedPouch)
            {
                badge = BAG_BOUGHT_SEED_POUCH;
            }
            else if (bag is ScrollHolder)
            {
                badge = BAG_BOUGHT_SCROLL_HOLDER;
            }
            else if (bag is WandHolster)
            {
                badge = BAG_BOUGHT_WAND_HOLSTER;
            }

            if (badge != null)
            {

                _local.Add(badge);

                if (!_local.Contains(ALL_BAGS_BOUGHT) && _local.Contains(BAG_BOUGHT_SCROLL_HOLDER) && _local.Contains(BAG_BOUGHT_SEED_POUCH) && _local.Contains(BAG_BOUGHT_WAND_HOLSTER))
                {

                    badge = ALL_BAGS_BOUGHT;
                    _local.Add(badge);
                    DisplayBadge(badge);
                }
            }
        }

        public static void ValidateAllItemsIdentified()
        {
            if (!_global.Contains(ALL_ITEMS_IDENTIFIED) && _global.Contains(ALL_POTIONS_IDENTIFIED) && _global.Contains(ALL_SCROLLS_IDENTIFIED) && _global.Contains(ALL_RINGS_IDENTIFIED) && _global.Contains(ALL_WANDS_IDENTIFIED))
            {

                Badge badge = ALL_ITEMS_IDENTIFIED;
                DisplayBadge(badge);
            }
        }

        public static void ValidateDeathFromFire()
        {
            Badge badge = DEATH_FROM_FIRE;
            _local.Add(badge);
            DisplayBadge(badge);

            ValidateYasd();
        }

        public static void ValidateDeathFromPoison()
        {
            Badge badge = DEATH_FROM_POISON;
            _local.Add(badge);
            DisplayBadge(badge);

            ValidateYasd();
        }

        public static void ValidateDeathFromGas()
        {
            Badge badge = DEATH_FROM_GAS;
            _local.Add(badge);
            DisplayBadge(badge);

            ValidateYasd();
        }

        public static void ValidateDeathFromHunger()
        {
            Badge badge = DEATH_FROM_HUNGER;
            _local.Add(badge);
            DisplayBadge(badge);

            ValidateYasd();
        }

        public static void ValidateDeathFromGlyph()
        {
            Badge badge = DEATH_FROM_GLYPH;
            _local.Add(badge);
            DisplayBadge(badge);
        }

        public static void ValidateDeathFromFalling()
        {
            Badge badge = DEATH_FROM_FALLING;
            _local.Add(badge);
            DisplayBadge(badge);
        }

        private static void ValidateYasd()
        {
            if (_global.Contains(DEATH_FROM_FIRE) && _global.Contains(DEATH_FROM_POISON) && _global.Contains(DEATH_FROM_GAS) && _global.Contains(DEATH_FROM_HUNGER))
            {

                Badge badge = YASD;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateBossSlain()
        {
            Badge badge = null;
            switch (Dungeon.Depth)
            {
                case 5:
                    badge = BOSS_SLAIN_1;
                    break;
                case 10:
                    badge = BOSS_SLAIN_2;
                    break;
                case 15:
                    badge = BOSS_SLAIN_3;
                    break;
                case 20:
                    badge = BOSS_SLAIN_4;
                    break;
            }

            if (badge != null)
            {
                _local.Add(badge);
                DisplayBadge(badge);

                if (badge == BOSS_SLAIN_1)
                {
                    switch (Dungeon.Hero.heroClass.Ordinal())
                    {
                        case HeroClassType.Warrior:
                            badge = BOSS_SLAIN_1_WARRIOR;
                            break;
                        case HeroClassType.Mage:
                            badge = BOSS_SLAIN_1_MAGE;
                            break;
                        case HeroClassType.Rogue:
                            badge = BOSS_SLAIN_1_ROGUE;
                            break;
                        case HeroClassType.Huntress:
                            badge = BOSS_SLAIN_1_HUNTRESS;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                _local.Add(badge);
                if (!_global.Contains(badge))
                {
                    _global.Add(badge);
                    _saveNeeded = true;
                }

                if (_global.Contains(BOSS_SLAIN_1_WARRIOR) && _global.Contains(BOSS_SLAIN_1_MAGE) && _global.Contains(BOSS_SLAIN_1_ROGUE) && _global.Contains(BOSS_SLAIN_1_HUNTRESS))
                {

                    badge = BOSS_SLAIN_1_ALL_CLASSES;
                    if (!_global.Contains(badge))
                    {
                        DisplayBadge(badge);
                        _global.Add(badge);
                        _saveNeeded = true;
                    }
                }
            }
            else if (badge == BOSS_SLAIN_3)
            {
                //switch (Dungeon.Hero.subClass)
                //{
                //    case GLADIATOR:
                //        badge = Badge.BOSS_SLAIN_3_GLADIATOR;
                //        break;
                //    case BERSERKER:
                //        badge = Badge.BOSS_SLAIN_3_BERSERKER;
                //        break;
                //    case WARLOCK:
                //        badge = Badge.BOSS_SLAIN_3_WARLOCK;
                //        break;
                //    case BATTLEMAGE:
                //        badge = Badge.BOSS_SLAIN_3_BATTLEMAGE;
                //        break;
                //    case FREERUNNER:
                //        badge = Badge.BOSS_SLAIN_3_FREERUNNER;
                //        break;
                //    case ASSASSIN:
                //        badge = Badge.BOSS_SLAIN_3_ASSASSIN;
                //        break;
                //    case SNIPER:
                //        badge = Badge.BOSS_SLAIN_3_SNIPER;
                //        break;
                //    case WARDEN:
                //        badge = Badge.BOSS_SLAIN_3_WARDEN;
                //        break;
                //    default:
                //        return;
                //}
                _local.Add(badge);
                if (!_global.Contains(badge))
                {
                    _global.Add(badge);
                    _saveNeeded = true;
                }

                if (_global.Contains(BOSS_SLAIN_3_GLADIATOR) && _global.Contains(BOSS_SLAIN_3_BERSERKER) && _global.Contains(BOSS_SLAIN_3_WARLOCK) && _global.Contains(BOSS_SLAIN_3_BATTLEMAGE) && _global.Contains(BOSS_SLAIN_3_FREERUNNER) && _global.Contains(BOSS_SLAIN_3_ASSASSIN) && _global.Contains(BOSS_SLAIN_3_SNIPER) && _global.Contains(BOSS_SLAIN_3_WARDEN))
                {

                    badge = BOSS_SLAIN_3_ALL_SUBCLASSES;
                    if (!_global.Contains(badge))
                    {
                        DisplayBadge(badge);
                        _global.Add(badge);
                        _saveNeeded = true;
                    }
                }
            }
        }


        public static void ValidateMastery()
        {

            Badge badge = null;
            switch (Dungeon.Hero.heroClass.Ordinal())
            {
                case HeroClassType.Warrior:
                    badge = MASTERY_WARRIOR;
                    break;
                case HeroClassType.Mage:
                    badge = MASTERY_MAGE;
                    break;
                case HeroClassType.Rogue:
                    badge = MASTERY_ROGUE;
                    break;
                case HeroClassType.Huntress:
                    badge = MASTERY_HUNTRESS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!_global.Contains(badge))
            {
                _global.Add(badge);
                _saveNeeded = true;
            }
        }

        public static void ValidateMasteryCombo(int n)
        {
            if (_local.Contains(MASTERY_COMBO) || n != 7)
                return;

            Badge badge = MASTERY_COMBO;
            _local.Add(badge);
            DisplayBadge(badge);
        }

        public static void ValidateRingOfHaggler()
        {
            if (!_local.Contains(RING_OF_HAGGLER) && new RingOfHaggler().IsKnown)
            {
                Badge badge = RING_OF_HAGGLER;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateRingOfThorns()
        {
            if (!_local.Contains(RING_OF_THORNS) && new RingOfThorns().IsKnown)
            {
                Badge badge = RING_OF_THORNS;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateRare(Mob mob)
        {
            Badge badge = null;
            if (mob is Albino)
            {
                badge = RARE_ALBINO;
            }
            else if (mob is Bandit)
            {
                badge = RARE_BANDIT;
            }
            else if (mob is Shielded)
            {
                badge = RARE_SHIELDED;
            }
            else if (mob is Senior)
            {
                badge = RARE_SENIOR;
            }
            else if (mob is Acidic)
            {
                badge = RARE_ACIDIC;
            }
            if (!_global.Contains(badge))
            {
                _global.Add(badge);
                _saveNeeded = true;
            }

            if (_global.Contains(RARE_ALBINO) && _global.Contains(RARE_BANDIT) && _global.Contains(RARE_SHIELDED) && _global.Contains(RARE_SENIOR) && _global.Contains(RARE_ACIDIC))
            {

                badge = RARE;
                DisplayBadge(badge);
            }
        }

        public static void ValidateVictory()
        {

            Badge badge = VICTORY;
            DisplayBadge(badge);
            switch (Dungeon.Hero.heroClass.Ordinal())
            {
                case HeroClassType.Warrior:
                    badge = VICTORY_WARRIOR;
                    break;
                case HeroClassType.Mage:
                    badge = VICTORY_MAGE;
                    break;
                case HeroClassType.Rogue:
                    badge = VICTORY_ROGUE;
                    break;
                case HeroClassType.Huntress:
                    badge = VICTORY_HUNTRESS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _local.Add(badge);
            if (!_global.Contains(badge))
            {
                _global.Add(badge);
                _saveNeeded = true;
            }

            if (_global.Contains(VICTORY_WARRIOR) && _global.Contains(VICTORY_MAGE) && _global.Contains(VICTORY_ROGUE) && _global.Contains(VICTORY_HUNTRESS))
            {

                badge = VICTORY_ALL_CLASSES;
                DisplayBadge(badge);
            }
        }

        public static void ValidateNoKilling()
        {
            if (!_local.Contains(NO_MONSTERS_SLAIN) && Statistics.CompletedWithNoKilling)
            {
                Badge badge = NO_MONSTERS_SLAIN;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateGrimWeapon()
        {
            if (!_local.Contains(GRIM_WEAPON))
            {
                Badge badge = GRIM_WEAPON;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateNightHunter()
        {
            if (!_local.Contains(NIGHT_HUNTER) && Statistics.NightHunt >= 15)
            {
                Badge badge = NIGHT_HUNTER;
                _local.Add(badge);
                DisplayBadge(badge);
            }
        }

        public static void ValidateSupporter()
        {
            _global.Add(SUPPORTER);
            _saveNeeded = true;

            PixelScene.ShowBadge(SUPPORTER);
        }

        public static void ValidateGamesPlayed()
        {
            Badge badge = null;
            if (Rankings.Instance.totalNumber >= 10)
            {
                badge = GAMES_PLAYED_1;
            }
            if (Rankings.Instance.totalNumber >= 100)
            {
                badge = GAMES_PLAYED_2;
            }
            if (Rankings.Instance.totalNumber >= 500)
            {
                badge = GAMES_PLAYED_3;
            }
            if (Rankings.Instance.totalNumber >= 2000)
            {
                badge = GAMES_PLAYED_4;
            }

            DisplayBadge(badge);
        }

        public static void ValidateHappyEnd()
        {
            DisplayBadge(HAPPY_END);
        }

        public static void ValidateChampion()
        {
            DisplayBadge(CHAMPION);
        }

        private static void DisplayBadge(Badge badge)
        {
            if (badge == null)
                return;

            if (_global.Contains(badge))
            {
                if (!badge.Meta)
                    GLog.Highlight("Badge endorsed: {0}", badge.Description);
            }
            else
            {
                _global.Add(badge);
                _saveNeeded = true;

                if (badge.Meta)
                    GLog.Highlight("New super badge: {0}", badge.Description);
                else
                    GLog.Highlight("New badge: {0}", badge.Description);
                PixelScene.ShowBadge(badge);
            }
        }

        public static bool IsUnlocked(Badge badge)
        {
            return _global.Contains(badge);
        }

        public static void Disown(Badge badge)
        {
            LoadGlobal();
            _global.Remove(badge);
            _saveNeeded = true;
        }

        public static IList<Badge> Filtered(bool global)
        {
            var filtered = new List<Badge>(global ? _global : _local);

            //if (!global)
            //{
            //    IEnumerator<Badge> iterator = filtered.GetEnumerator();
            //    while (iterator.MoveNext())
            //    {
            //        Badge badge = iterator.Current;
            //        if (badge.Meta)
            //        {
            //            iterator.remove();
            //        }
            //    }
            //}

            //leaveBest = new Badge(filtered, Badge.MONSTERS_SLAIN_1, Badge.MONSTERS_SLAIN_2, Badge.MONSTERS_SLAIN_3, Badge.MONSTERS_SLAIN_4);
            //leaveBest = new Badge(filtered, Badge.GOLD_COLLECTED_1, Badge.GOLD_COLLECTED_2, Badge.GOLD_COLLECTED_3, Badge.GOLD_COLLECTED_4);
            //leaveBest = new Badge(filtered, Badge.BOSS_SLAIN_1, Badge.BOSS_SLAIN_2, Badge.BOSS_SLAIN_3, Badge.BOSS_SLAIN_4);
            //leaveBest = new Badge(filtered, Badge.LEVEL_REACHED_1, Badge.LEVEL_REACHED_2, Badge.LEVEL_REACHED_3, Badge.LEVEL_REACHED_4);
            //leaveBest = new Badge(filtered, Badge.STRENGTH_ATTAINED_1, Badge.STRENGTH_ATTAINED_2, Badge.STRENGTH_ATTAINED_3, Badge.STRENGTH_ATTAINED_4);
            //leaveBest = new Badge(filtered, Badge.FOOD_EATEN_1, Badge.FOOD_EATEN_2, Badge.FOOD_EATEN_3, Badge.FOOD_EATEN_4);
            //leaveBest = new Badge(filtered, Badge.ITEM_LEVEL_1, Badge.ITEM_LEVEL_2, Badge.ITEM_LEVEL_3, Badge.ITEM_LEVEL_4);
            //leaveBest = new Badge(filtered, Badge.POTIONS_COOKED_1, Badge.POTIONS_COOKED_2, Badge.POTIONS_COOKED_3, Badge.POTIONS_COOKED_4);
            //leaveBest = new Badge(filtered, Badge.BOSS_SLAIN_1_ALL_CLASSES, Badge.BOSS_SLAIN_3_ALL_SUBCLASSES);
            //leaveBest = new Badge(filtered, Badge.DEATH_FROM_FIRE, Badge.YASD);
            //leaveBest = new Badge(filtered, Badge.DEATH_FROM_GAS, Badge.YASD);
            //leaveBest = new Badge(filtered, Badge.DEATH_FROM_HUNGER, Badge.YASD);
            //leaveBest = new Badge(filtered, Badge.DEATH_FROM_POISON, Badge.YASD);
            //leaveBest = new Badge(filtered, Badge.VICTORY, Badge.VICTORY_ALL_CLASSES);
            //leaveBest = new Badge(filtered, Badge.GAMES_PLAYED_1, Badge.GAMES_PLAYED_2, Badge.GAMES_PLAYED_3, Badge.GAMES_PLAYED_4);

            var list = new List<Badge>(filtered);
            list.Sort();

            return list;
        }

        private static void LeaveBest(HashSet<Badge> list, params Badge[] badges)
        {
            for (int i = badges.Length - 1; i > 0; i--)
            {
                if (list.Contains(badges[i]))
                {
                    for (int j = 0; j < i; j++)
                    {
                        list.Remove(badges[j]);
                    }
                    break;
                }
            }
        }
    }
}