using System;

namespace sharpdungeon.actors.mobs
{
    public class Bestiary
    {
        public static Mob Mob(int depth)
        {

            var cl = MobClass(depth);
            try
            {
                return (Mob)Activator.CreateInstance(cl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Mob Mutable(int depth)
        {
            var cl = MobClass(depth);

            if (pdsharp.utils.Random.Int(30) == 0)
            {
                if (cl == typeof(Rat))
                {
                    cl = typeof(Albino);
                }
                else if (cl == typeof(Thief))
                {
                    cl = typeof(Bandit);
                }
                else if (cl == typeof(Brute))
                {
                    cl = typeof(Shielded);
                }
                else if (cl == typeof(Monk))
                {
                    cl = typeof(Senior);
                }
                else if (cl == typeof(Scorpio))
                {
                    cl = typeof(Acidic);
                }
            }

            try
            {
                return (Mob)Activator.CreateInstance(cl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Type MobClass(int depth)
        {
            float[] chances;

            Type[] classes;

            switch (depth)
            {
                case 1:
                    chances = new float[] { 1 };

                    classes = new[] { typeof(Rat) };
                    break;
                case 2:
                    chances = new float[] { 1, 1 };

                    classes = new[] { typeof(Rat), typeof(Gnoll) };
                    break;
                case 3:
                    chances = new[] { 1, 2, 1, 0.02f };

                    classes = new[] { typeof(Rat), typeof(Gnoll), typeof(Crab), typeof(Swarm) };
                    break;
                case 4:
                    chances = new[] { 1, 2, 3, 0.02f, 0.01f, 0.01f };

                    classes = new[] { typeof(Rat), typeof(Gnoll), typeof(Crab), typeof(Swarm), typeof(Skeleton), typeof(Thief) };
                    break;

                case 5:
                    chances = new float[] { 1 };

                    classes = new[] { typeof(Goo) };
                    break;

                case 6:
                    chances = new[] { 4, 2, 1, 0.2f };

                    classes = new[] { typeof(Skeleton), typeof(Thief), typeof(Swarm), typeof(Shaman) };
                    break;
                case 7:
                    chances = new float[] { 3, 1, 1, 1 };

                    classes = new[] { typeof(Skeleton), typeof(Shaman), typeof(Thief), typeof(Swarm) };
                    break;
                case 8:
                    chances = new[] { 3, 2, 1, 1, 1, 0.02f };

                    classes = new[] { typeof(Skeleton), typeof(Shaman), typeof(Gnoll), typeof(Thief), typeof(Swarm), typeof(Bat) };
                    break;
                case 9:
                    chances = new[] { 3, 3, 1, 1, 0.02f, 0.01f };

                    classes = new[] { typeof(Skeleton), typeof(Shaman), typeof(Thief), typeof(Swarm), typeof(Bat), typeof(Brute) };
                    break;

                case 10:
                    chances = new float[] { 1 };

                    classes = new Type[] { typeof(Tengu) };
                    break;

                case 11:
                    chances = new float[] { 1, 0.2f };

                    classes = new Type[] { typeof(Bat), typeof(Brute) };
                    break;
                case 12:
                    chances = new float[] { 1, 1, 0.2f };

                    //ORIGINAL LINE: classes = new Type[]{ Bat.class, Brute.class, Spinner.class };
                    classes = new Type[] { typeof(Bat), typeof(Brute), typeof(Spinner) };
                    break;
                case 13:
                    chances = new float[] { 1, 3, 1, 1, 0.02f };

                    //ORIGINAL LINE: classes = new Type[]{ Bat.class, Brute.class, Shaman.class, Spinner.class, Elemental.class };
                    classes = new Type[] { typeof(Bat), typeof(Brute), typeof(Shaman), typeof(Spinner), typeof(Elemental) };
                    break;
                case 14:
                    chances = new float[] { 1, 3, 1, 4, 0.02f, 0.01f };

                    //ORIGINAL LINE: classes = new Type[]{ Bat.class, Brute.class, Shaman.class, Spinner.class, Elemental.class, Monk.class };
                    classes = new Type[] { typeof(Bat), typeof(Brute), typeof(Shaman), typeof(Spinner), typeof(Elemental), typeof(Monk) };
                    break;

                case 15:
                    chances = new float[] { 1 };

                    //ORIGINAL LINE: classes = new Type[]{ DM300.class };
                    classes = new Type[] { typeof(DM300) };
                    break;

                case 16:
                    chances = new float[] { 1, 1, 0.2f };

                    //ORIGINAL LINE: classes = new Type[]{ Elemental.class, Warlock.class, Monk.class };
                    classes = new Type[] { typeof(Elemental), typeof(Warlock), typeof(Monk) };
                    break;
                case 17:
                    chances = new float[] { 1, 1, 1 };

                    //ORIGINAL LINE: classes = new Type[]{ Elemental.class, Monk.class, Warlock.class };
                    classes = new Type[] { typeof(Elemental), typeof(Monk), typeof(Warlock) };
                    break;
                case 18:
                    chances = new float[] { 1, 2, 1, 1 };

                    //ORIGINAL LINE: classes = new Type[]{ Elemental.class, Monk.class, Golem.class, Warlock.class };
                    classes = new Type[] { typeof(Elemental), typeof(Monk), typeof(Golem), typeof(Warlock) };
                    break;
                case 19:
                    chances = new float[] { 1, 2, 3, 1, 0.02f };

                    //ORIGINAL LINE: classes = new Type[]{ Elemental.class, Monk.class, Golem.class, Warlock.class, Succubus.class };
                    classes = new Type[] { typeof(Elemental), typeof(Monk), typeof(Golem), typeof(Warlock), typeof(Succubus) };
                    break;

                case 20:
                    chances = new float[] { 1 };

                    //ORIGINAL LINE: classes = new Type[]{ King.class };
                    classes = new Type[] { typeof(King) };
                    break;

                case 22:
                    chances = new float[] { 1, 1 };

                    //ORIGINAL LINE: classes = new Type[]{ Succubus.class, Eye.class };
                    classes = new Type[] { typeof(Succubus), typeof(Eye) };
                    break;
                case 23:
                    chances = new float[] { 1, 2, 1 };

                    //ORIGINAL LINE: classes = new Type[]{ Succubus.class, Eye.class, Scorpio.class };
                    classes = new Type[] { typeof(Succubus), typeof(Eye), typeof(Scorpio) };
                    break;
                case 24:
                    chances = new float[] { 1, 2, 3 };

                    //ORIGINAL LINE: classes = new Type[]{ Succubus.class, Eye.class, Scorpio.class };
                    classes = new Type[] { typeof(Succubus), typeof(Eye), typeof(Scorpio) };
                    break;

                case 25:
                    chances = new float[] { 1 };

                    classes = new Type[] { typeof(Yog) };
                    break;

                default:
                    chances = new float[] { 1 };

                    classes = new Type[] { typeof(Eye) };
                    break;
            }

            return classes[pdsharp.utils.Random.Chances(chances)];
        }

        public static bool IsUnique(Character mob)
        {
            return mob is Goo || mob is Tengu || mob is DM300 || mob is King || mob is Yog;
        }
    }
}