using System.Collections.Generic;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects.particles;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Wraith : Mob
    {
        private const float SpawnDelay = 2f;

        private int _level;

        public Wraith()
        {
            Name = "wraith";
            SpriteClass = typeof(WraithSprite);

            HP = HT = 1;
            Exp = 0;

            Flying = true;
        }

        private const string Level = "Level";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Level, _level);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _level = bundle.GetInt(Level);
            AdjustStats(_level);
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(1, 3 + _level);
        }

        public override int AttackSkill(Character target)
        {
            return 10 + _level;
        }

        public virtual void AdjustStats(int level)
        {
            _level = level;
            defenseSkill = AttackSkill(null) * 5;
            EnemySeen = true;
        }

        public override string DefenseVerb()
        {
            return "evaded";
        }

        public override bool Reset()
        {
            State = WANDERING;
            return true;
        }

        public override string Description()
        {
            return "A wraith is a vengeful spirit of a sinner, whose grave or tomb was disturbed. " + "Being an ethereal entity, it is very hard to hit with a regular weapon.";
        }

        public static void SpawnAround(int pos)
        {
            foreach (var n in levels.Level.NEIGHBOURS4)
            {
                var cell = pos + n;
                if (levels.Level.passable[cell] && FindChar(cell) == null)
                    SpawnAt(cell);
            }
        }

        public static Wraith SpawnAt(int pos)
        {
            if (!levels.Level.passable[pos] || FindChar(pos) != null) 
                return null;

            var w = new Wraith();
            w.AdjustStats(Dungeon.Depth);
            w.pos = pos;
            w.State = w.HUNTING;
            GameScene.Add(w, SpawnDelay);

            w.Sprite.Alpha(0);
            w.Sprite.Parent.Add(new AlphaTweener(w.Sprite, 1, 0.5f));

            w.Sprite.Emitter().Burst(ShadowParticle.Curse, 5);

            return w;
        }

        private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
        static Wraith()
        {
            IMMUNITIES.Add(typeof(Death));
            IMMUNITIES.Add(typeof(Terror));
        }

        public override HashSet<System.Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}