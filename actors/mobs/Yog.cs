using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.items.keys;
using sharpdungeon.utils;
using Fire = sharpdungeon.items.weapon.enchantments.Fire;
using Poison = sharpdungeon.items.weapon.enchantments.Poison;
using sharpdungeon.effects.particles;

namespace sharpdungeon.actors.mobs
{
    public class Yog : Mob
    {
        private const string TxtDesc = "Yog-Dzewa is an Old God, a powerful entity from the realms of chaos. A century ago, the ancient dwarves " + "barely won the war against its army of demons, but were unable to kill the god itself. Instead, they then " + "imprisoned it in the halls below their city, believing it to be too weak to rise ever again.";

        private static int _fistsCount;

        public Yog()
        {
            Name = "Yog-Dzewa";
            SpriteClass = typeof(YogSprite);

            HP = HT = 300;

            Exp = 50;

            State = PASSIVE;
        }

        public virtual void SpawnFists()
        {
            var fist1 = new RottingFist();
            var fist2 = new BurningFist();

            do
            {
                fist1.pos = pos + Level.NEIGHBOURS8[Random.Int(8)];
                fist2.pos = pos + Level.NEIGHBOURS8[Random.Int(8)];
            }
            while (!Level.passable[fist1.pos] || !Level.passable[fist2.pos] || fist1.pos == fist2.pos);

            GameScene.Add(fist1);
            GameScene.Add(fist2);
        }

        public override void Damage(int dmg, object src)
        {
            if (_fistsCount > 0)
            {
                foreach (Mob mob in Dungeon.Level.mobs.Where(mob => mob is BurningFist || mob is RottingFist))
                    mob.Beckon(pos);

                dmg >>= _fistsCount;
            }

            base.Damage(dmg, src);
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            var spawnPoints = new List<int>();

            for (var i = 0; i < Level.NEIGHBOURS8.Length; i++)
            {
                var p = pos + Level.NEIGHBOURS8[i];
                if (FindChar(p) == null && (Level.passable[p] || Level.avoid[p]))
                    spawnPoints.Add(p);
            }

            if (spawnPoints.Count <= 0)
                return base.DefenseProc(enemy, damage);

            var larva = new Larva();
            larva.pos = Random.Element(spawnPoints);

            GameScene.Add(larva);
            AddDelayed(new Pushing(larva, pos, larva.pos), -1);

            return base.DefenseProc(enemy, damage);
        }

        public override void Beckon(int cell)
        {
        }

        public override void Die(object cause)
        {
            //TODO: Original mobs.Clone ?!?
            foreach (var mob in Dungeon.Level.mobs.Where(mob => mob is BurningFist || mob is RottingFist))
                mob.Die(cause);

            GameScene.BossSlain();
            Dungeon.Level.Drop(new SkeletonKey(), pos).Sprite.Drop();
            base.Die(cause);

            Yell("...");
        }

        public override void Notice()
        {
            base.Notice();
            Yell("Hope is an illusion...");
        }

        public override string Description()
        {
            return TxtDesc;

        }

        private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
        private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();

        static Yog()
        {
            IMMUNITIES.Add(typeof(Death));
            IMMUNITIES.Add(typeof(Terror));
            IMMUNITIES.Add(typeof(Amok));
            IMMUNITIES.Add(typeof(Charm));
            IMMUNITIES.Add(typeof(Sleep));
            IMMUNITIES.Add(typeof(Burning));
            IMMUNITIES.Add(typeof(ToxicGas));
            IMMUNITIES.Add(typeof(ScrollOfPsionicBlast));
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            IMMUNITIES.Add(typeof(Amok));
            IMMUNITIES.Add(typeof(Sleep));
            IMMUNITIES.Add(typeof(Terror));
            IMMUNITIES.Add(typeof(Poison));
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            IMMUNITIES.Add(typeof(Amok));
            IMMUNITIES.Add(typeof(Sleep));
            IMMUNITIES.Add(typeof(Terror));
            IMMUNITIES.Add(typeof(Burning));
        }

        public override HashSet<System.Type> Immunities()
        {
            return IMMUNITIES;
        }

        public class RottingFist : Mob
        {
            private const int Regeneration = 4;

            public RottingFist()
            {
                _fistsCount++;

                Name = "rotting fist";
                SpriteClass = typeof(RottingFistSprite);

                HP = HT = 300;
                defenseSkill = 25;

                Exp = 0;

                State = WANDERING;
            }

            public override void Die(object cause)
            {
                base.Die(cause);
                _fistsCount--;
            }

            public override int AttackSkill(Character target)
            {
                return 36;
            }

            public override int DamageRoll()
            {
                return Random.NormalIntRange(24, 36);
            }

            public override int Dr()
            {
                return 15;
            }

            public override int AttackProc(Character enemy, int damage)
            {
                if (Random.Int(3) != 0)
                    return damage;

                buffs.Buff.Affect<Ooze>(enemy);
                enemy.Sprite.Burst(Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00), 5);

                return damage;
            }

            protected override bool Act()
            {
                if (!Level.water[pos] || HP >= HT)
                    return base.Act();

                Sprite.Emitter().Burst(ShadowParticle.Up, 2);
                HP += Regeneration;

                return base.Act();
            }

            public override string Description()
            {
                return TxtDesc;
            }

            private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();
            public override HashSet<System.Type> Resistances()
            {
                return RESISTANCES;
            }

            private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
            public override HashSet<System.Type> Immunities()
            {
                return IMMUNITIES;
            }
        }

        public class BurningFist : Mob
        {
            public BurningFist()
            {
                _fistsCount++;

                Name = "burning fist";
                SpriteClass = typeof(BurningFistSprite);

                HP = HT = 200;
                defenseSkill = 25;

                Exp = 0;

                State = WANDERING;
            }

            public override void Die(object cause)
            {
                base.Die(cause);
                _fistsCount--;
            }

            public override int AttackSkill(Character target)
            {
                return 36;
            }

            public override int DamageRoll()
            {
                return Random.NormalIntRange(20, 32);
            }

            public override int Dr()
            {
                return 15;
            }

            protected internal override bool CanAttack(Character enemy)
            {
                return Ballistica.Cast(pos, enemy.pos, false, true) == enemy.pos;
            }

            public override bool Attack(Character enemy)
            {
                if (Level.Adjacent(pos, enemy.pos))
                    return base.Attack(enemy);

                Spend(AttackDelay());

                if (Hit(this, enemy, true))
                {
                    var dmg = DamageRoll();
                    enemy.Damage(dmg, this);

                    enemy.Sprite.BloodBurstA(Sprite.Center(), dmg);
                    enemy.Sprite.Flash();

                    if (enemy.IsAlive || enemy != Dungeon.Hero)
                        return true;

                    Dungeon.Fail(Utils.Format(ResultDescriptions.BOSS, Name, Dungeon.Depth));
                    GLog.Negative(TxtKill, Name);
                    return true;
                }

                enemy.Sprite.ShowStatus(CharSprite.Neutral, enemy.DefenseVerb());
                return false;
            }

            protected override bool Act()
            {
                foreach (var neighbour in Level.NEIGHBOURS9)
                    GameScene.Add(Blob.Seed(pos + neighbour, 2, typeof(Fire)));

                return base.Act();
            }

            public override string Description()
            {
                return TxtDesc;
            }


            private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();
            public override HashSet<System.Type> Resistances()
            {
                return RESISTANCES;
            }

            private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
            public override HashSet<System.Type> Immunities()
            {
                return IMMUNITIES;
            }
        }

        public class Larva : Mob
        {
            public Larva()
            {
                Name = "god's larva";
                SpriteClass = typeof(LarvaSprite);

                HP = HT = 25;
                defenseSkill = 20;

                Exp = 0;

                State = HUNTING;
            }

            public override int AttackSkill(Character target)
            {
                return 30;
            }

            public override int DamageRoll()
            {
                return Random.NormalIntRange(15, 20);
            }

            public override int Dr()
            {
                return 8;
            }

            public override string Description()
            {
                return TxtDesc;

            }
        }
    }
}