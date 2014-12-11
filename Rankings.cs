using System;
using System.Collections.Generic;
using System.Linq;
using Java.IO;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon
{
    public class Rankings
    {
        public static Rankings Instance;

        public static int TABLE_SIZE = 6;

        public static String RANKINGS_FILE = "rankings.dat";

        public static String DETAILS_FILE = "game_{0}.dat";

        public List<Record> records;
        public int lastRecord;
        public int totalNumber;

        private static String RECORDS = "records";

        private static String LATEST = "latest";

        private static String TOTAL = "total";

        //private static IComparator scoreComparator = Comperator<Record>.Default;

        public void Submit(bool win)
        {
            Load();

            var rec = new Record();

            rec.info = Dungeon.ResultDescription;
            rec.win = win;
            rec.heroClass = Dungeon.Hero.heroClass;
            rec.armorTier = Dungeon.Hero.Tier();
            rec.score = Score(win);

            var gameFile = Utils.Format(DETAILS_FILE, DateTime.Now);
            try
            {
                Dungeon.SaveGame(gameFile);
                rec.gameFile = gameFile;
            }
            catch (IOException)
            {
                rec.gameFile = "";
            }

            records.Add(rec);
            records = records.OrderByDescending(o => o.score).ToList();

            lastRecord = records.IndexOf(rec);
            var size = records.Count;
            if (size > TABLE_SIZE)
            {
                Record removedGame;
                if (lastRecord == size - 1)
                {
                    removedGame = records[size - 2];
                    records.Remove(removedGame);
                    lastRecord--;
                }
                else
                {
                    removedGame = records[size - 1];
                    records.Remove(removedGame);
                }

                if (removedGame.gameFile.Length > 0)
                    Game.Instance.DeleteFile(removedGame.gameFile);
            }

            totalNumber++;

            Badge.ValidateGamesPlayed();

            Save();
        }

        private static int Score(bool win)
        {
            return (Statistics.GoldCollected + Dungeon.Hero.Lvl * Dungeon.Depth * 100) * (win ? 2 : 1);
        }

        public void Save()
        {
            var bundle = new Bundle();
            bundle.Put(RECORDS, records);
            bundle.Put(LATEST, lastRecord);
            bundle.Put(TOTAL, totalNumber);

            try
            {
                var output = Game.Instance.OpenFileOutput(RANKINGS_FILE, Android.Content.FileCreationMode.Private);
                Bundle.Write(bundle, output);
                output.Close();
            }
            catch (Exception)
            {
            }
        }

        public void Load()
        {
            if (records != null)
                return;

            records = new List<Record>();

            try
            {
                var input = Game.Instance.OpenFileInput(RANKINGS_FILE);
                var bundle = Bundle.Read(input);
                input.Close();

                foreach (var record in bundle.GetCollection(RECORDS))
                    records.Add((Record)record);

                lastRecord = bundle.GetInt(LATEST);

                totalNumber = bundle.GetInt(TOTAL);
                if (totalNumber == 0)
                    totalNumber = records.Count;
            }
            catch (Exception)
            {
            }
        }

        public int Compare(Record lhs, Record rhs)
        {
            return Math.Sign(rhs.score - lhs.score);
        }
    }

    public class Record : Bundlable
    {

        private static String REASON = "reason";

        public static String WIN = "win";

        private static String SCORE = "score";

        private static String TIER = "tier";

        private static String GAME = "gameFile";


        public String info;

        public bool win;


        public HeroClass heroClass;

        public int armorTier;


        public int score;


        public String gameFile;

        public void RestoreFromBundle(pdsharp.utils.Bundle bundle)
        {
            info = bundle.GetString(REASON);
            win = bundle.GetBoolean(WIN);
            score = bundle.GetInt(SCORE);

            heroClass = HeroClass.ReStoreInBundle(bundle);
            armorTier = bundle.GetInt(TIER);

            gameFile = bundle.GetString(GAME);
        }

        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put(REASON, info);
            bundle.Put(WIN, win);
            bundle.Put(SCORE, score);

            heroClass.StoreInBundle(bundle);
            bundle.Put(TIER, armorTier);

            bundle.Put(GAME, gameFile);
        }
    }
}