using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Java.Lang;
using Java.Util;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.utils;
using sharpdungeon.windows;
using Character = sharpdungeon.actors.Character;
using Exception = System.Exception;
using Math = System.Math;
using Random = pdsharp.utils.Random;

namespace sharpdungeon
{
    public class Dungeon
    {
        private const string NoTips = "The bitmapText  is indecipherable...";
        private static readonly string[] Tips = { "Don't overestimate your strength, use weapons and armor you can handle.", "Not All doors in the dungeon are visible at first sight. If you are stuck, search for hidden doors.", "Remember, that raising your strength is not the only way to access better equipment, you can go " + "the other way lowering its strength requirement with Scrolls of Upgrade.", "You can spend your gold in shops on deeper levels of the dungeon. The first one is on the 6th Level.", "Beware of Goo!", "Pixel-Mart - All you need for successful adventure!", "Identify your potions and scrolls as soon as possible. Don't put it off to the moment " + "when you actually need them.", "Being hungry doesn't hurt, but starving does hurt.", "Surprise DoAttack has a better chance to hit. For example, you can ambush your enemy behind " + "a closed door when you know it is approaching.", "Don't let The Tengu out!", "Pixel-Mart. Spend money. Live longer.", "When you're attacked by several monsters at the same time, try to retreat behind a door.", "If you are burning, you can't put out the fire in the water while levitating.", "There is no sense in possessing more than one Ankh at the same time, because you will lose them upon resurrecting.", "DANGER! Heavy machinery can cause injury, loss of limbs or death!", "Pixel-Mart. A safer life in dungeon.", "When you Upgrade an enchanted weapon, there is a chance to destroy that enchantment.", "In a Well of Transmutation you can Get an item, that cannot be obtained otherwise.", "The only way to enchant a weapon is by upgrading it with a Scroll of Weapon Upgrade.", "No weapons allowed in the presence of His Majesty!", "Pixel-Mart. Special prices for demon hunters!", "The bitmapText is written in demonic language.", "The bitmapText is written in demonic language.", "The bitmapText is written in demonic language." };

        private const string TxtDeadEnd = "What are you doing here?!";

        public static int PotionOfStrength;
        public static int ScrollsOfUpgrade;
        public static int ArcaneStyli;
        public static bool DewVial; // true if the dew vial can be spawned
        public static int Transmutation; // depth number for a well of transmutation

        public static int Challenges;

        public static Hero Hero;
        public static Level Level;

        public static Item Quickslot;

        public static int Depth;
        public static int Gold;
        // Reason of death
        public static string ResultDescription;

        public static HashSet<int?> Chapters;

        // Hero's field of view
        public static bool[] Visible = new bool[Level.Length];

        public static bool NightMode;

        public static void Init()
        {
#if !Console
            Challenges = PixelDungeon.Challenges();
#endif

            Actor.Clear();

            PathFinder.SetMapSize(Level.Width, Level.Height);

            Scroll.InitLabels();
            Potion.InitColors();
            Wand.InitWoods();
            Ring.InitGems();

            Statistics.Reset();
            Journal.Reset();

            Depth = 0;
            Gold = 0;

            PotionOfStrength = 0;
            ScrollsOfUpgrade = 0;
            ArcaneStyli = 0;
            DewVial = true;
            Transmutation = Random.IntRange(6, 14);

            Chapters = new HashSet<int?>();

            Ghost.Quest.reset();
            Wandmaker.Quest.Reset();
            Blacksmith.Quest.Reset();
            Imp.Quest.Reset();

            Room.ShuffleTypes();

            Hero = new Hero();
            Hero.Live();

            Badge.Reset();

            StartScene.curClass.InitHero(Hero);
        }

        public static bool IsChallenged(int mask)
        {
            return (Challenges & mask) != 0;
        }

        public static Level NewLevel()
        {
            Level = null;
            Actor.Clear();

            Depth++;
            if (Depth > Statistics.DeepestFloor)
            {
                Statistics.DeepestFloor = Depth;

                if (Statistics.QualifiedForNoKilling)
                    Statistics.CompletedWithNoKilling = true;
                else
                    Statistics.CompletedWithNoKilling = false;
            }

            Arrays.Fill(Visible, false);

            Level level;
            switch (Depth)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    level = new SewerLevel();
                    break;
                case 5:
                    level = new SewerBossLevel();
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    level = new PrisonLevel();
                    break;
                case 10:
                    level = new PrisonBossLevel();
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    level = new CavesLevel();
                    break;
                case 15:
                    level = new CavesBossLevel();
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    level = new CityLevel();
                    break;
                case 20:
                    level = new CityBossLevel();
                    break;
                case 21:
                    level = new LastShopLevel();
                    break;
                case 22:
                case 23:
                case 24:
                    level = new HallsLevel();
                    break;
                case 25:
                    level = new HallsBossLevel();
                    break;
                case 26:
                    level = new LastLevel();
                    break;
                default:
                    level = new DeadEndLevel();
                    Statistics.DeepestFloor--;
                    break;
            }

            level.Create();

            Statistics.QualifiedForNoKilling = !BossLevel();

            return level;
        }

        public static void ResetLevel()
        {
            Actor.Clear();

            Arrays.Fill(Visible, false);

            Level.Reset();
            SwitchLevel(Level, Level.entrance);
        }

        public static string Tip()
        {
            if (Level is DeadEndLevel)
                return TxtDeadEnd;

            var index = Depth - 1;

            return index < Tips.Length ? Tips[index] : NoTips;
        }

        public static bool ShopOnLevel()
        {
            return Depth == 6 || Depth == 11 || Depth == 16;
        }

        public static bool BossLevel()
        {
            return BossLevel(Depth);
        }

        public static bool BossLevel(int depth)
        {
            return depth == 5 || depth == 10 || depth == 15 || depth == 20 || depth == 25;
        }

        public static void SwitchLevel(Level level, int pos)
        {
            NightMode = DateTime.Now.Hour < 7;

            Level = level;
            Actor.Init();

            var respawner = level.Respawner();
            if (respawner != null)
                Actor.Add(level.Respawner());

            Hero.pos = pos != -1 ? pos : level.exit;

            var light = Hero.Buff<Light>();
            Hero.viewDistance = light == null ? level.viewDistance : Math.Max(Light.Distance, level.viewDistance);

            Observe();
        }

        public static bool PosNeeded()
        {
            int[] quota = { 4, 2, 9, 4, 14, 6, 19, 8, 24, 9 };
            return Chance(quota, PotionOfStrength);
        }

        public static bool SoeNeeded()
        {
            int[] quota = { 5, 3, 10, 6, 15, 9, 20, 12, 25, 13 };
            return Chance(quota, ScrollsOfUpgrade);
        }

        private static bool Chance(int[] quota, int number)
        {
            for (var i = 0; i < quota.Length; i += 2)
            {
                var qDepth = quota[i];
                if (Depth > qDepth)
                    continue;

                var qNumber = quota[i + 1];
                return Random.Float() < (float)(qNumber - number) / (qDepth - Depth + 1);
            }

            return false;
        }

        public static bool AsNeeded()
        {
            return Random.Int(12 * (1 + ArcaneStyli)) < Depth;
        }

        private const string RgGameFile = "game.dat";
        private const string RgDepthFile = "depth{0}.dat";

        private const string WrGameFile = "warrior.dat";
        private const string WrDepthFile = "warrior{0}.dat";

        private const string MgGameFile = "mage.dat";
        private const string MgDepthFile = "mage{0}.dat";

        private const string RnGameFile = "ranger.dat";
        private const string RnDepthFile = "ranger{0}.dat";

        private const string Version = "version";
        private const string CHALLENGES = "challenges";
        private const string HERO = "hero";
        private const string GOLD = "gold";
        private const string DEPTH = "depth";
        private const string QUICKSLOT = "quickslot";
        private const string LEVEL = "Level";
        private const string POS = "potionsOfStrength";
        private const string SOU = "scrollsOfEnhancement";
        private const string AS = "arcaneStyli";
        private const string DV = "dewVial";
        private const string WT = "transmutation";
        private const string CHAPTERS = "chapters";
        private const string QUESTS = "quests";
        private const string BADGES = "badges";

        public static string GameFile(HeroClass cl)
        {
            if (cl == HeroClass.Warrior)
                return WrGameFile;

            if (cl == HeroClass.Mage)
                return MgGameFile;

            if (cl == HeroClass.Huntress)
                return RnGameFile;
            
            return RgGameFile;
        }

        private static string DepthFile(HeroClass cl)
        {
            if (cl == HeroClass.Warrior)
                return WrDepthFile;

            if (cl == HeroClass.Mage)
                return MgDepthFile;

            if (cl == HeroClass.Huntress)
                return RnDepthFile;
            
            return RgDepthFile;
        }

        public static void SaveGame(string fileName)
        {
            try
            {
                var bundle = new Bundle();

                bundle.Put(Version, Game.version);
                bundle.Put(CHALLENGES, Challenges);
                bundle.Put(HERO, Hero);
                bundle.Put(GOLD, Gold);
                bundle.Put(DEPTH, Depth);

                bundle.Put(POS, PotionOfStrength);
                bundle.Put(SOU, ScrollsOfUpgrade);
                bundle.Put(AS, ArcaneStyli);
                bundle.Put(DV, DewVial);
                bundle.Put(WT, Transmutation);

                var count = 0;
                var ids = new int?[Chapters.Count];
                foreach (var id in Chapters)
                    ids[count++] = id;

                bundle.Put(CHAPTERS, ids);

                var quests = new Bundle();
                Ghost.Quest.StoreInBundle(quests);
                Wandmaker.Quest.StoreInBundle(quests);
                Blacksmith.Quest.StoreInBundle(quests);
                Imp.Quest.StoreInBundle(quests);
                bundle.Put(QUESTS, quests);

                Room.StoreRoomsInBundle(bundle);

                Statistics.StoreInBundle(bundle);
                Journal.StoreInBundle(bundle);

                if (Quickslot != null)
                    bundle.Put(QUICKSLOT, Quickslot.Name);

                Scroll.Save(bundle);
                Potion.Save(bundle);
                Wand.Save(bundle);
                Ring.Save(bundle);

                var badges = new Bundle();
                Badge.SaveLocal(badges);
                bundle.Put(BADGES, badges);

                var output = Game.Instance.OpenFileOutput(fileName, FileCreationMode.Private);
                Bundle.Write(bundle, output);
                output.Close();
            }
            catch (Exception)
            {
                GamesInProgress.Unknown = Hero.heroClass;
            }
        }

        public static void SaveLevel()
        {
            var bundle = new Bundle();
            bundle.Put(LEVEL, Level);

            var output = Game.Instance.OpenFileOutput(Utils.Format(DepthFile(Hero.heroClass), Depth), FileCreationMode.Private);
            Bundle.Write(bundle, output);
            output.Close();
        }

        public static void SaveAll()
        {
            if (Hero.IsAlive)
            {
                Actor.FixTime();
                SaveGame(GameFile(Hero.heroClass));
                SaveLevel();

                GamesInProgress.Set(Hero.heroClass, Depth, Hero.Lvl);
            }
            else
                if (WndResurrect.Instance != null)
                {
                    WndResurrect.Instance.Hide();
                    Hero.ReallyDie(WndResurrect.CauseOfDeath);
                }
        }

        public static void LoadGame(HeroClass cl)
        {
            LoadGame(GameFile(cl), true);
        }

        public static void LoadGame(string fileName)
        {
            LoadGame(fileName, false);
        }

        public static void LoadGame(string fileName, bool fullLoad)
        {
            var bundle = GameBundle(fileName);

            Challenges = bundle.GetInt(CHALLENGES);

            Level = null;
            Depth = -1;

            if (fullLoad)
                PathFinder.SetMapSize(Level.Width, Level.Height);

            Scroll.Restore(bundle);
            Potion.Restore(bundle);
            Wand.Restore(bundle);
            Ring.Restore(bundle);

            PotionOfStrength = bundle.GetInt(POS);
            ScrollsOfUpgrade = bundle.GetInt(SOU);
            ArcaneStyli = bundle.GetInt(AS);
            DewVial = bundle.GetBoolean(DV);
            Transmutation = bundle.GetInt(WT);

            if (fullLoad)
            {
                Chapters = new HashSet<int?>();
                var ids = bundle.GetIntArray(CHAPTERS);
                if (ids != null)
                    foreach (var id in ids)
                        Chapters.Add(id);

                var quests = bundle.GetBundle(QUESTS);
                if (!quests.IsNull)
                {
                    Ghost.Quest.RestoreFromBundle(quests);
                    Wandmaker.Quest.RestoreFromBundle(quests);
                    Blacksmith.Quest.RestoreFromBundle(quests);
                    Imp.Quest.RestoreFromBundle(quests);
                }
                else
                {
                    Ghost.Quest.reset();
                    Wandmaker.Quest.Reset();
                    Blacksmith.Quest.Reset();
                    Imp.Quest.Reset();
                }

                Room.RestoreRoomsFromBundle(bundle);
            }

            var badges = bundle.GetBundle(BADGES);
            if (!badges.IsNull)
                Badge.LoadLocal(badges);
            else
                Badge.Reset();

            var qsClass = bundle.GetString(QUICKSLOT);
            if (qsClass != null)
            {
                try
                {
                    Quickslot = (Item)Activator.CreateInstance(Type.GetType(qsClass));
                }
                catch (ClassNotFoundException)
                {
                }
            }
            else
                Quickslot = null;


            var version = bundle.GetString(Version);

            Hero = null;
            Hero = (Hero)bundle.Get(HERO);

            Gold = bundle.GetInt(GOLD);
            Depth = bundle.GetInt(DEPTH);

            Statistics.RestoreFromBundle(bundle);
            Journal.RestoreFromBundle(bundle);
        }

        public static Level LoadLevel(HeroClass cl)
        {
            Level = null;
            Actor.Clear();

            var input = Game.Instance.OpenFileInput(Utils.Format(DepthFile(cl), Depth));
            var bundle = Bundle.Read(input);
            input.Close();

            return (Level)bundle.Get("Level");
        }

        public static void DeleteGame(HeroClass cl, bool deleteLevels)
        {
            Game.Instance.DeleteFile(GameFile(cl));

            if (deleteLevels)
            {
                var localDepth = 1;
                while (Game.Instance.DeleteFile(Utils.Format(DepthFile(cl), localDepth)))
                    localDepth++;
            }

            GamesInProgress.Delete(cl);
        }

        public static Bundle GameBundle(string fileName)
        {
            var input = Game.Instance.OpenFileInput(fileName);
            var bundle = Bundle.Read(input);
            input.Close();

            return bundle;
        }

        public static void Preview(GamesInProgress.Info info, Bundle bundle)
        {
            info.Depth = bundle.GetInt(DEPTH);
            if (info.Depth == -1)
                info.Depth = bundle.GetInt("maxDepth"); // FIXME

            Hero.Preview(info, bundle.GetBundle(HERO));
        }

        public static void Fail(string desc)
        {
            ResultDescription = desc;
            if (Hero.Belongings.GetItem<Ankh>() == null)
                Rankings.Instance.Submit(false);
        }

        public static void Win(string desc)
        {
            if (Challenges != 0)
                Badge.ValidateChampion();

            ResultDescription = desc;
            Rankings.Instance.Submit(true);
        }

        public static void Observe()
        {
            if (Level == null)
                return;

            Level.UpdateFieldOfView(Hero);
            Array.Copy(Level.fieldOfView, 0, Visible, 0, Visible.Length);

            BArray.or(Level.visited, Visible, Level.visited);

            GameScene.AfterObserve();
        }

        private static readonly bool[] Passable = new bool[Level.Length];

        public static int FindPath(Character ch, int from, int to, bool[] pass, bool[] visible)
        {
            if (Level.Adjacent(from, to))
                return Actor.FindChar(to) == null && (pass[to] || Level.avoid[to]) ? to : -1;

            if (ch.Flying || ch.Buff<Amok>() != null)
                BArray.or(pass, Level.avoid, Passable);
            else
                Array.Copy(pass, 0, Passable, 0, Level.Length);

            foreach (int pos in Actor.All.OfType<Character>().Select(actor => (actor).pos).Where(pos => visible[pos]))
                Passable[pos] = false;

            return PathFinder.GetStep(from, to, Passable);
        }

        public static int Flee(Character ch, int cur, int from, bool[] pass, bool[] visible)
        {
            if (ch.Flying)
                BArray.or(pass, Level.avoid, Passable);
            else
                Array.Copy(pass, 0, Passable, 0, Level.Length);

            foreach (var pos in Actor.All.OfType<Character>().Select(c => c.pos).Where(pos => visible[pos]))
                Passable[pos] = false;

            Passable[cur] = true;

            return PathFinder.GetStepBack(cur, from, Passable);
        }
    }
}