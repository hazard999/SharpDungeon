using System;
using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.sprites;
using sharpdungeon.levels;
using sharpdungeon.utils;

namespace sharpdungeon.actors.mobs
{
    public abstract class Mob : Character
    {
        protected Mob()
        {
            var sleepState = new AIStateSleeping(this);
            Sleepeing = sleepState;
            HUNTING = new Hunting(this);
            WANDERING = new Wandering(this);
            FLEEING = new Fleeing(this);
            PASSIVE = new Passive(this);
            State = sleepState;
        }

        private const string TxtDied = "You hear something died in the distance";

        protected internal const string TxtNotice1 = "?!";
        protected internal const string TxtRage = "#$%^";
        protected internal const string TxtExp = "{0}EXP";

        public IAiState Sleepeing;
        public IAiState HUNTING;
        public IAiState WANDERING;
        public IAiState FLEEING;
        public IAiState PASSIVE;
        public IAiState State;

        public Type SpriteClass;

        protected internal int Target = -1;

        protected internal int defenseSkill = 0;

        protected internal int Exp = 1;
        protected internal int MaxLvl = 30;

        protected internal Character Enemy;
        protected internal bool EnemySeen;
        protected internal bool Alerted;

        protected internal const float TimeToWakeUp = 1f;

        public bool Hostile = true;

        // Unreachable target
        //TODO: FIX: public static Mob DUMMY = new Mob { pos = -1 };
        public static Mob Dummy; // = new Mob { pos = -1 };

        private const string STATE = "state";
        private const string TARGET = "target";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);

            if (State == Sleepeing)
                bundle.Put(STATE, AIStateSleeping.Tag);
            else
                if (State == WANDERING)
                    bundle.Put(STATE, Wandering.Tag);
                else
                    if (State == HUNTING)
                        bundle.Put(STATE, Hunting.TAG);
                    else
                        if (State == FLEEING)
                            bundle.Put(STATE, Fleeing.Tag);
                        else
                            if (State == PASSIVE)
                                bundle.Put(STATE, Passive.Tag);
            bundle.Put(TARGET, Target);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            var state = bundle.GetString(STATE);
            if (state.Equals(AIStateSleeping.Tag))
                State = Sleepeing;
            else
                if (state.Equals(Wandering.Tag))
                    State = WANDERING;
                else
                    if (state.Equals(Hunting.TAG))
                        State = HUNTING;
                    else
                        if (state.Equals(Fleeing.Tag))
                            State = FLEEING;
                        else
                            if (state.Equals(Passive.Tag))
                                State = PASSIVE;

            Target = bundle.GetInt(TARGET);
        }

        public override CharSprite Sprite
        {
            get
            {
                if (_sprite != null)
                    return _sprite;

                try
                {
                    _sprite = (CharSprite)Activator.CreateInstance(SpriteClass);
                }
                catch (Exception)
                {
                }

                return _sprite;
            }
        }

        protected override bool Act()
        {
            base.Act();

            var justAlerted = Alerted;
            Alerted = false;

            base.Sprite.HideAlert();

            if (Paralysed)
            {
                EnemySeen = false;
                Spend(Tick);
                return true;
            }

            Enemy = ChooseEnemy();

            var enemyInFov = Enemy.IsAlive && Level.fieldOfView[Enemy.pos] && Enemy.invisible <= 0;

            return State.Act(enemyInFov, justAlerted);
        }

        protected internal virtual Character ChooseEnemy()
        {
            if (Buff<Amok>() != null)
                if (Enemy == Dungeon.Hero || Enemy == null)
                {
                    var enemies = new List<Mob>();

                    foreach (var mob in Dungeon.Level.mobs)
                        if (mob != this && Level.fieldOfView[mob.pos])
                            enemies.Add(mob);

                    if (enemies.Count > 0)
                        return pdsharp.utils.Random.Element(enemies.ToArray());
                }
                else
                    return Enemy;

            var terror = Buff<Terror>();
            if (terror != null)
                return terror.Source;

            return Dungeon.Hero;
        }

        protected internal virtual bool MoveSprite(int from, int to)
        {
            if (base.Sprite.Visible && (Dungeon.Visible[from] || Dungeon.Visible[to]))
            {
                base.Sprite.Move(from, to);
                return true;
            }

            base.Sprite.Place(to);
            return true;
        }

        public override void Add(Buff buff)
        {
            Actor.Add(buff);
            if (buff is Amok)
            {
                if (base.Sprite != null)
                    base.Sprite.ShowStatus(CharSprite.Negative, TxtRage);

                State = HUNTING;
            }
            else
                if (buff is Terror)
                    State = FLEEING;
                else
                    if (buff is Sleep)
                    {
                        if (base.Sprite != null)
                            new Flare(4, 32).Color(0x44ffff, true).Show(base.Sprite, 2f);

                        State = Sleepeing;
                        Postpone(Sleep.Sws);
                    }
        }

        public override void Remove(Buff buff)
        {
            base.Remove(buff);
            if (!(buff is Terror))
                return;

            base.Sprite.ShowStatus(CharSprite.Negative, TxtRage);
            State = HUNTING;
        }

        protected internal virtual bool CanAttack(Character enemy)
        {
            return Level.Adjacent(pos, enemy.pos) && !Pacified;
        }

        protected internal virtual bool GetCloser(int target)
        {
            if (Rooted)
                return false;

            var step = Dungeon.FindPath(this, pos, target, Level.passable, Level.fieldOfView);

            if (step == -1)
                return false;

            Move(step);
            return true;
        }

        protected internal virtual bool GetFurther(int target)
        {
            var step = Dungeon.Flee(this, pos, target, Level.passable, Level.fieldOfView);

            if (step == -1)
                return false;

            Move(step);
            return true;
        }

        public override void Move(int step)
        {
            base.Move(step);

            if (!Flying)
                Dungeon.Level.MobPress(this);
        }

        protected internal virtual float AttackDelay()
        {
            return 1f;
        }

        protected internal virtual bool DoAttack(Character enemy)
        {
            var visible = Dungeon.Visible[pos];

            if (visible)
                Sprite.DoAttack(enemy.pos);
            else
                Attack(enemy);

            Spend(AttackDelay());

            return !visible;
        }

        public override void OnAttackComplete()
        {
            Attack(Enemy);
            base.OnAttackComplete();
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return EnemySeen && !Paralysed ? defenseSkill : 0;
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            if (!EnemySeen && enemy == Dungeon.Hero && ((Hero)enemy).subClass == HeroSubClass.ASSASSIN)
            {
                damage += pdsharp.utils.Random.Int(1, damage);
                Wound.Hit(this);
            }
            return damage;
        }

        public override void Damage(int dmg, object src)
        {

            Terror.Recover(this);

            if (State == Sleepeing)
            {
                State = WANDERING;
            }
            Alerted = true;

            base.Damage(dmg, src);
        }


        public override void Destroy()
        {

            base.Destroy();

            Dungeon.Level.mobs.Remove(this);

            if (!Dungeon.Hero.IsAlive)
                return;

            if (Hostile)
            {
                Statistics.EnemiesSlain++;
                Badge.ValidateMonstersSlain();
                Statistics.QualifiedForNoKilling = false;

                if (Dungeon.NightMode)
                    Statistics.NightHunt++;
                else
                    Statistics.NightHunt = 0;

                Badge.ValidateNightHunter();
            }

            if (Dungeon.Hero.Lvl > MaxLvl || Exp <= 0)
                return;

            Dungeon.Hero.Sprite.ShowStatus(CharSprite.Positive, TxtExp, Exp);
            Dungeon.Hero.EarnExp(Exp);
        }

        public override void Die(object cause)
        {
            base.Die(cause);

            if (Dungeon.Hero.Lvl <= MaxLvl + 2)
                DropLoot();

            if (Dungeon.Hero.IsAlive && !Dungeon.Visible[pos])
                GLog.Information(TxtDied);
        }

        protected internal object loot = null;
        protected internal float lootChance = 0;
        private CharSprite _sprite;

        protected internal virtual void DropLoot()
        {
            if (loot == null || !(pdsharp.utils.Random.Float() < lootChance)) 
                return;

            Item item;
            if (loot is Generator.Category)
                item = Generator.Random((Generator.Category)loot);
            else if (loot is Type)
                item = Generator.Random((Type)loot);
            else
                item = (Item)loot;
            Dungeon.Level.Drop(item, pos).Sprite.Drop();
        }

        public virtual bool Reset()
        {
            return false;
        }

        public virtual void Beckon(int cell)
        {
            Notice();

            if (State != HUNTING)
                State = WANDERING;

            Target = cell;
        }

        public virtual string Description()
        {
            return "Real description is coming soon!";
        }

        public virtual void Notice()
        {
            Sprite.ShowAlert();
        }

        public virtual void Yell(string str)
        {
            GLog.Negative("{0}: \"{1}\" ", Name, str);
        }

        public override int AttackSkill(Character target)
        {
            return 0;
        }
    }
}