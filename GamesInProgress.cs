using System.Collections.Generic;
using sharpdungeon.actors.hero;

namespace sharpdungeon
{
    public class GamesInProgress
    {
        private static readonly Dictionary<HeroClass, Info> State = new Dictionary<HeroClass, Info>();

        public static Info Check(HeroClass cl)
        {
            if (State.ContainsKey(cl))
                return State[cl];

            Info info;
            try
            {
                var bundle = Dungeon.GameBundle(Dungeon.GameFile(cl));
                info = new Info();
                Dungeon.Preview(info, bundle);

            }
            catch
            {
                info = null;
            }

            State.Add(cl, info);
            return info;
        }

        public static void Set(HeroClass cl, int depth, int level)
        {
            var info = new Info();
            info.Depth = depth;
            info.Level = level;

            if (State.ContainsKey(cl))
                State[cl] = info;
            else
                State.Add(cl, info);
        }

        public static HeroClass Unknown
        {
            set
            {
                State.Remove(value);
            }
        }

        public static void Delete(HeroClass cl)
        {
            if (State.ContainsKey(cl))
                State[cl] = null;
            else
                State.Add(cl, null);
        }

        public class Info
        {
            public int Depth;
            public int Level;
        }
    }

}