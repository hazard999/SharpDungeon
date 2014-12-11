using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.levels.features;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.actors
{
    public abstract class Character : Actor
    {
        protected internal const string TxtHit = "{0} hit {1}";
        protected internal const string TxtKill = "{0} killed you...";
        protected internal const string TxtDefeat = "{0} defeated {1}";

        private const string TxtYouMissed = "{0} {1} your attack";
        private const string TxtSmbMissed = "{0} {1} {2}'s attack";

        private const string TxtOutOfParalysis = "The pain snapped {1} out of paralysis";

        public int pos;

        public virtual CharSprite Sprite { get; set; }

        public string Name = "mob";

        public int HT;
        public int HP;

        protected internal float baseSpeed = 1;

        public bool Paralysed = false;
        public bool Pacified = false;
        public bool Rooted = false;
        public bool Flying = false;
        public int invisible = 0;

        public int viewDistance = 8;

        private readonly IList<Buff> _buffs = new List<Buff>();

        protected override bool Act()
        {
            Dungeon.Level.UpdateFieldOfView(this);
            return false;
        }

        private const string POS = "pos";
        private const string TAG_HP = "HP";
        private const string TAG_HT = "HT";
        private const string BUFFS = "buffs";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);

            bundle.Put(POS, pos);
            bundle.Put(TAG_HP, HP);
            bundle.Put(TAG_HT, HT);
            bundle.Put(BUFFS, _buffs);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            pos = bundle.GetInt(POS);
            HP = bundle.GetInt(TAG_HP);
            HT = bundle.GetInt(TAG_HT);

            foreach (var b in bundle.GetCollection(BUFFS).Where(b => b != null))
                ((Buff)b).AttachTo(this);
        }

        public virtual bool Attack(Character enemy)
        {
            var visibleFight = Dungeon.Visible[pos] || Dungeon.Visible[enemy.pos];

            if (Hit(this, enemy, false))
            {
                if (visibleFight)
                    GLog.Information(TxtHit, Name, enemy.Name);

                // FIXME
                var dr = this is Hero && ((Hero)this).RangedWeapon != null && ((Hero)this).subClass == HeroSubClass.SNIPER ? 0 : Random.IntRange(0, enemy.Dr());

                var dmg = DamageRoll();
                var effectiveDamage = Math.Max(dmg - dr, 0);

                effectiveDamage = AttackProc(enemy, effectiveDamage);
                effectiveDamage = enemy.DefenseProc(this, effectiveDamage);
                enemy.Damage(effectiveDamage, this);

                if (visibleFight)
                    Sample.Instance.Play(Assets.SND_HIT, 1, 1, Random.Float(0.8f, 1.25f));

                if (enemy == Dungeon.Hero)
                    Dungeon.Hero.Interrupt();

                enemy.Sprite.BloodBurstA(Sprite.Center(), effectiveDamage);
                enemy.Sprite.Flash();

                if (!enemy.IsAlive && visibleFight)
                {
                    if (enemy == Dungeon.Hero)
                    {
                        if (Dungeon.Hero.KillerGlyph != null)
                        {
                            Dungeon.Fail(Utils.Format(ResultDescriptions.GLYPH, Dungeon.Hero.KillerGlyph.Name(), Dungeon.Depth));
                            GLog.Negative(TxtKill, Dungeon.Hero.KillerGlyph.Name());
                        }
                        else
                        {
                            if (Bestiary.IsUnique(this))
                                Dungeon.Fail(Utils.Format(ResultDescriptions.BOSS, Name, Dungeon.Depth));
                            else
                                Dungeon.Fail(Utils.Format(ResultDescriptions.MOB, Utils.Indefinite(Name), Dungeon.Depth));

                            GLog.Negative(TxtKill, Name);
                        }
                    }
                    else
                        GLog.Information(TxtDefeat, Name, enemy.Name);
                }

                return true;

            }

            if (!visibleFight)
                return false;

            var defense = enemy.DefenseVerb();
            enemy.Sprite.ShowStatus(CharSprite.Neutral, defense);
            if (this == Dungeon.Hero)
                GLog.Information(TxtYouMissed, enemy.Name, defense);
            else
                GLog.Information(TxtSmbMissed, enemy.Name, defense, Name);

            Sample.Instance.Play(Assets.SND_MISS);

            return false;
        }

        public static bool Hit(Character attacker, Character defender, bool magic)
        {
            float acuRoll = Random.Float(attacker.AttackSkill(defender));
            float defRoll = Random.Float(defender.DefenseSkill(attacker));
            return (magic ? acuRoll * 2 : acuRoll) >= defRoll;
        }

        public virtual int AttackSkill(Character target)
        {
            return 0;
        }

        public virtual int DefenseSkill(Character localEnemy)
        {
            return 0;
        }

        public virtual string DefenseVerb()
        {
            return "dodged";
        }

        public virtual int Dr()
        {
            return 0;
        }

        public virtual int DamageRoll()
        {
            return 1;
        }

        public virtual int AttackProc(Character enemy, int damage)
        {
            return damage;
        }

        public virtual int DefenseProc(Character enemy, int damage)
        {
            return damage;
        }

        public virtual float Speed()
        {
            return Buff<Cripple>() == null ? baseSpeed : baseSpeed * 0.5f;
        }

        public virtual void Damage(int dmg, object src)
        {
            if (HP <= 0)
                return;

            buffs.Buff.Detach<Frost>(this);

            var srcClass = src.GetType();
            if (Immunities().Contains(srcClass))
                dmg = 0;
            else
                if (Resistances().Contains(srcClass))
                    dmg = Random.IntRange(0, dmg);

            if (Buff<Paralysis>() != null)
                if (Random.Int(dmg) >= Random.Int(HP))
                {
                    buffs.Buff.Detach<Paralysis>(this);
                    if (Dungeon.Visible[pos])
                        GLog.Information(TxtOutOfParalysis, Name);
                }

            HP -= dmg;

            if (dmg > 0 || src is Character)
                Sprite.ShowStatus(HP > HT / 2 ? CharSprite.Warning : CharSprite.Negative, dmg.ToString());

            if (HP <= 0)
                Die(src);
        }

        public virtual void Destroy()
        {
            HP = 0;
            Remove(this);
            FreeCell(pos);
        }

        public virtual void Die(object src)
        {
            Destroy();
            Sprite.DoDie();
        }

        public virtual bool IsAlive
        {
            get { return HP > 0; }
        }

        protected internal override void Spend(float time)
        {
            var timeScale = 1f;

            if (Buff<Slow>() != null)
                timeScale *= 0.5f;

            if (Buff<Speed>() != null)
                timeScale *= 2.0f;

            base.Spend(time / timeScale);
        }

        public virtual IEnumerable<Buff> Buffs()
        {
            return _buffs;
        }

        public virtual IEnumerable<T1> Buffs<T1>() where T1 : Buff
        {
            return _buffs.OfType<T1>();
        }

        public virtual T Buff<T>() where T : Buff
        {
            return _buffs.OfType<T>().FirstOrDefault();
        }

        public virtual Buff Buff(Buff c)
        {
            return _buffs.FirstOrDefault(b => c.GetType() == b.GetType());
        }

        public virtual void Add(Buff buff)
        {
            _buffs.Add(buff);
            Actor.Add(buff);

            if (Sprite == null)
                return;

            if (buff is Poison)
            {
                CellEmitter.Center(pos).Burst(PoisonParticle.Splash, 5);
                Sprite.ShowStatus(CharSprite.Negative, "poisoned");
            }
            else if (buff is Amok)
                Sprite.ShowStatus(CharSprite.Negative, "amok");
            else if (buff is Slow)
            {
                Sprite.ShowStatus(CharSprite.Negative, "slowed");
            }
            else if (buff is MindVision)
            {
                Sprite.ShowStatus(CharSprite.Positive, "mind");
                Sprite.ShowStatus(CharSprite.Positive, "vision");
            }
            else if (buff is Paralysis)
            {
                Sprite.Add(CharSprite.State.Paralysed);
                Sprite.ShowStatus(CharSprite.Negative, "paralysed");
            }
            else if (buff is Terror)
                Sprite.ShowStatus(CharSprite.Negative, "frightened");
            else if (buff is Roots)
                Sprite.ShowStatus(CharSprite.Negative, "rooted");
            else if (buff is Cripple)
                Sprite.ShowStatus(CharSprite.Negative, "crippled");
            else if (buff is Bleeding)
                Sprite.ShowStatus(CharSprite.Negative, "bleeding");
            else if (buff is Vertigo)
                Sprite.ShowStatus(CharSprite.Negative, "dizzy");
            else if (buff is Sleep)
                Sprite.Idle();
            else if (buff is Burning)
                Sprite.Add(CharSprite.State.Burning);
            else if (buff is Levitation)
                Sprite.Add(CharSprite.State.Levitating);
            else if (buff is Frost)
                Sprite.Add(CharSprite.State.Frozen);
            else if (buff is Invisibility)
            {
                if (!(buff is Shadows))
                    Sprite.ShowStatus(CharSprite.Positive, "invisible");
                Sprite.Add(CharSprite.State.Invisible);
            }
        }

        public virtual void Remove(Buff buff)
        {
            _buffs.Remove(buff);
            Actor.Remove(buff);

            if (buff is Burning)
                Sprite.Remove(CharSprite.State.Burning);
            else if (buff is Levitation)
                Sprite.Remove(CharSprite.State.Levitating);
            else if (buff is Invisibility && invisible <= 0)
                Sprite.Remove(CharSprite.State.Invisible);
            else if (buff is Paralysis)
                Sprite.Remove(CharSprite.State.Paralysed);
            else if (buff is Frost)
                Sprite.Remove(CharSprite.State.Frozen);
        }

        public virtual void Remove<T1>() where T1 : Buff
        {
            foreach (var buff in Buffs<T1>())
                Remove(buff);
        }

        protected internal override void OnRemove()
        {
            foreach (var buff in _buffs)
                buff.Detach();
        }

        public virtual void UpdateSpriteState()
        {
            foreach (var buff in _buffs)
            {
                if (buff is Burning)
                    Sprite.Add(CharSprite.State.Burning);
                else if (buff is Levitation)
                    Sprite.Add(CharSprite.State.Levitating);
                else if (buff is Invisibility)
                    Sprite.Add(CharSprite.State.Invisible);
                else if (buff is Paralysis)
                    Sprite.Add(CharSprite.State.Paralysed);
                else if (buff is Frost)
                    Sprite.Add(CharSprite.State.Frozen);
                else if (buff is Light)
                    Sprite.Add(CharSprite.State.Illuminated);
            }
        }

        public virtual int Stealth()
        {
            return 0;
        }

        public virtual void Move(int step)
        {
            if (Buff<Vertigo>() != null)
            {
                var candidates = new List<int>();
                foreach (var dir in Level.NEIGHBOURS8)
                {
                    var p = pos + dir;
                    if ((Level.passable[p] || Level.avoid[p]) && FindChar(p) == null)
                        candidates.Add(p);
                }

                step = Random.Element(candidates.ToArray());
            }

            if (Dungeon.Level.map[pos] == Terrain.OPEN_DOOR)
                Door.Leave(pos);

            pos = step;

            if (Flying && Dungeon.Level.map[pos] == Terrain.DOOR)
                Door.Enter(pos);

            if (this != Dungeon.Hero)
                Sprite.Visible = Dungeon.Visible[pos];
        }

        public virtual int Distance(Character other)
        {
            return Level.Distance(pos, other.pos);
        }

        public virtual void OnMotionComplete()
        {
            Next();
        }

        public virtual void OnAttackComplete()
        {
            Next();
        }

        public virtual void OnOperateComplete()
        {
            Next();
        }

        private static readonly HashSet<Type> Empty = new HashSet<Type>();

        public virtual HashSet<Type> Resistances()
        {
            return Empty;
        }

        public virtual HashSet<Type> Immunities()
        {
            return Empty;
        }
    }
}