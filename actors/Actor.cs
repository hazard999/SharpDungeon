using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.levels;

namespace sharpdungeon.actors
{
    public abstract class Actor : Bundlable
    {
        public const float Tick = 1f;

        private float _time;

        protected abstract bool Act();

        protected internal virtual void Spend(float time)
        {
            _time += time;
        }

        protected internal virtual void Postpone(float time)
        {
            if (_time < _now + time)
                _time = _now + time;
        }

        protected internal virtual float Cooldown()
        {
            return _time - _now;
        }

        protected internal virtual void Deactivate()
        {
            _time = float.MaxValue;
        }

        protected internal virtual void OnAdd()
        {
        }

        protected internal virtual void OnRemove()
        {
        }

        private const string TIME = "time";

        public virtual void StoreInBundle(Bundle bundle)
        {
            bundle.Put(TIME, _time);
        }

        public virtual void RestoreFromBundle(Bundle bundle)
        {
            _time = bundle.GetFloat(TIME);
        }

        // **********************
        // *** Static members ***

        private static readonly List<Actor> AllActors = new List<Actor>();
        private static Actor _current;

        private static float _now;

        private static Character[] _characters = new Character[Level.Length];

        public static void Clear()
        {
            _now = 0;

            AllActors.Clear();
        }

        public static void FixTime()
        {
            if (Dungeon.Hero != null && AllActors.Contains(Dungeon.Hero))
                Statistics.Duration += _now;

            var min = All.Select(a => a._time).Concat(new[] { float.MaxValue }).Min();
            foreach (var a in All)
                a._time -= min;

            _now = 0;
        }

        public static void Init()
        {
            Add(Dungeon.Hero);
            //AddDelayed(Dungeon.Hero, float.MinValue);

            foreach (var mob in Dungeon.Level.mobs)
                Add(mob);

            foreach (var blob in Dungeon.Level.Blobs.Values)
                Add(blob);

            _current = null;
        }

        public static void OccupyCell(Character ch)
        {
            _characters[ch.pos] = ch;
        }

        public static void FreeCell(int pos)
        {
            _characters[pos] = null;
        }

        //protected
        public virtual void Next()
        {
            if (_current == this)
                _current = null;
        }

        public static void Process()
        {
            if (_current != null)
                return;

            bool doNext;

            do
            {
                _now = float.MaxValue;
                _current = null;

                _characters = new Character[Level.Length];

                foreach (var actor in All)
                {
                    if (actor._time < _now)
                    {
                        _now = actor._time;
                        _current = actor;
                    }

                    if (!(actor is Character))
                        continue;

                    var ch = (Character)actor;
                    _characters[ch.pos] = ch;
                }

                if (_current != null)
                {
                    if (_current is Character && ((Character) _current).Sprite.IsMoving)
                    {
                        // If it's character's turn to act, but its sprite 
                        // is moving, wait till the movement is over
                        _current = null;
                        break;
                    }

                    doNext = _current.Act();

                    if (!doNext || Dungeon.Hero.IsAlive)
                        continue;

                    doNext = false;
                    _current = null;
                }
                else
                    doNext = false;
            } 
            while (doNext);
        }

        public static void Add(Actor actor)
        {
            Add(actor, _now);
        }

        public static void AddDelayed(Actor actor, float delay)
        {
            Add(actor, _now + delay);
        }

        private static void Add(Actor actor, float time)
        {
            if (All.Contains(actor))
                return;

            All.Add(actor);
            actor._time += time; // (+=) => (=) ?
            actor.OnAdd();

            var character = actor as Character;
            
            if (character == null) 
                return;

            var ch = character;
            _characters[ch.pos] = ch;
            foreach (var buff in ch.Buffs())
            {
                All.Add(buff);
                buff.OnAdd();
            }
        }

        public static void Remove(Actor actor)
        {
            if (actor == null)
                return;

            All.Remove(actor);
            actor.OnRemove();
        }

        public static Character FindChar(int pos)
        {
            return _characters[pos];
        }

        public static List<Actor> All
        {
            get { return AllActors; }
        }
    }
}