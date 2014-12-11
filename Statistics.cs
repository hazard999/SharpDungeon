using pdsharp.utils;

namespace sharpdungeon
{
    public class Statistics
    {
        public static int GoldCollected;
        public static int DeepestFloor;
        public static int EnemiesSlain;
        public static int FoodEaten;
        public static int PotionsCooked;
        public static int PiranhasKilled;
        public static int NightHunt;
        public static int AnkhsUsed;

        public static float Duration;

        public static bool QualifiedForNoKilling = false;
        public static bool CompletedWithNoKilling = false;

        public static bool AmuletObtained = false;

        public static void Reset()
        {

            GoldCollected = 0;
            DeepestFloor = 0;
            EnemiesSlain = 0;
            FoodEaten = 0;
            PotionsCooked = 0;
            PiranhasKilled = 0;
            NightHunt = 0;
            AnkhsUsed = 0;

            Duration = 0;

            QualifiedForNoKilling = false;

            AmuletObtained = false;

        }

        private const string GOLD = "score";
        private const string DEEPEST = "maxDepth";
        private const string SLAIN = "enemiesSlain";
        private const string FOOD = "foodEaten";
        private const string ALCHEMY = "potionsCooked";
        private const string PIRANHAS = "priranhas";
        private const string NIGHT = "nightHunt";
        private const string ANKHS = "ankhsUsed";
        private const string DURATION = "duration";
        private const string AMULET = "amuletObtained";

        public static void StoreInBundle(Bundle bundle)
        {
            bundle.Put(GOLD, GoldCollected);
            bundle.Put(DEEPEST, DeepestFloor);
            bundle.Put(SLAIN, EnemiesSlain);
            bundle.Put(FOOD, FoodEaten);
            bundle.Put(ALCHEMY, PotionsCooked);
            bundle.Put(PIRANHAS, PiranhasKilled);
            bundle.Put(NIGHT, NightHunt);
            bundle.Put(ANKHS, AnkhsUsed);
            bundle.Put(DURATION, Duration);
            bundle.Put(AMULET, AmuletObtained);
        }

        public static void RestoreFromBundle(Bundle bundle)
        {
            GoldCollected = bundle.GetInt(GOLD);
            DeepestFloor = bundle.GetInt(DEEPEST);
            EnemiesSlain = bundle.GetInt(SLAIN);
            FoodEaten = bundle.GetInt(FOOD);
            PotionsCooked = bundle.GetInt(ALCHEMY);
            PiranhasKilled = bundle.GetInt(PIRANHAS);
            NightHunt = bundle.GetInt(NIGHT);
            AnkhsUsed = bundle.GetInt(ANKHS);
            Duration = bundle.GetFloat(DURATION);
            AmuletObtained = bundle.GetBoolean(AMULET);
        }

    }

}