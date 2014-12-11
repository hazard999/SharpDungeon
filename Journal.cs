using pdsharp.utils;
using System;
using System.Collections.Generic;

namespace sharpdungeon
{
    public class Journal
    {
        public class Feature
        {
            public static Feature WELL_OF_HEALTH = new Feature("Well of Health");
            public static Feature WELL_OF_AWARENESS = new Feature("Well of Awareness");
            public static Feature WELL_OF_TRANSMUTATION = new Feature("Well of Transmutation");
            public static Feature ALCHEMY = new Feature("Alchemy pot");
            public static Feature GARDEN = new Feature("Garden");
            public static Feature STATUE = new Feature("Animated statue");
            public static Feature GHOST = new Feature("Sad ghost");
            public static Feature WANDMAKER = new Feature("Old wandmaker");
            public static Feature TROLL = new Feature("Troll blacksmith");
            public static Feature IMP = new Feature("Ambitious imp");

            public String desc;

            public Feature(String desc)
            {
                this.desc = desc;
            }
        }

        public class Record : IComparable<Record>, Bundlable
        {

            private const string FEATURE = "feature";
            private const string DEPTH = "depth";

            public Feature feature;
            public int depth;

            public Record()
            {
            }

            public Record(Feature feature, int depth)
            {
                this.feature = feature;
                this.depth = depth;
            }

            public int CompareTo(Record another)
            {
                return another.depth - depth;
            }

            public void RestoreFromBundle(Bundle bundle)
            {
                feature = new Feature(bundle.GetString(FEATURE));
                depth = bundle.GetInt(DEPTH);
            }

            public void StoreInBundle(Bundle bundle)
            {
                bundle.Put(FEATURE, feature.ToString());
                bundle.Put(DEPTH, depth);
            }
        }

        public static List<Record> Records;

        public static void Reset()
        {
            Records = new List<Journal.Record>();
        }

        private const string JOURNAL = "journal";

        public static void StoreInBundle(Bundle bundle)
        {
            bundle.Put(JOURNAL, Records);
        }

        public static void RestoreFromBundle(Bundle bundle)
        {
            Records = new List<Record>();
            foreach (var rec in bundle.GetCollection(JOURNAL))
                Records.Add((Record)rec);
        }

        public static void Add(Feature feature)
        {
            var size = Records.Count;
            for (var i = 0; i < size; i++)
            {
                var rec = Records[i];
                if (rec.feature == feature && rec.depth == Dungeon.Depth)
                    return;
            }

            Records.Add(new Record(feature, Dungeon.Depth));
        }

        public static void Remove(Feature feature)
        {
            var size = Records.Count;
            for (var i = 0; i < size; i++)
            {
                var rec = Records[i];
                if (rec.feature != feature || rec.depth != Dungeon.Depth)
                    continue;
                Records.Remove(rec);
                return;
            }
        }
    }
}