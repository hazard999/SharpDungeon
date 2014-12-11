using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.levels;

namespace sharpdungeon.actors.mobs.npcs
{
    public class MirrorImage : NPC
    {
        public MirrorImage()
        {
            Name = "mirror Image";
            SpriteClass = typeof(MirrorSprite);

            State = HUNTING;

            Enemy = Dummy;
        }

        public int tier;

        private int _attack;
        private int _damage;

        private const string Tier = "tier";
        private const string ATTACK = "DoAttack";
        private const string DAMAGE = "damage";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Tier, tier);
            bundle.Put(ATTACK, _attack);
            bundle.Put(DAMAGE, _damage);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            tier = bundle.GetInt(Tier);
            _attack = bundle.GetInt(ATTACK);
            _damage = bundle.GetInt(DAMAGE);
        }

        public virtual void Duplicate(Hero hero)
        {
            tier = hero.Tier();
            _attack = hero.AttackSkill(hero);
            _damage = hero.DamageRoll();
        }

        public override int AttackSkill(Character target)
        {
            return _attack;
        }

        public override int DamageRoll()
        {
            return _damage;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            var dmg = base.AttackProc(enemy, damage);

            Destroy();
            this.Sprite.DoDie();

            return dmg;
        }

        protected internal override Character ChooseEnemy()
        {
            if (Enemy != Dummy && Enemy.IsAlive)
                return Enemy;

            var enemies = Dungeon.Level.mobs.Where(mob => mob.Hostile && Level.fieldOfView[mob.pos]).ToList();

            Enemy = enemies.Count > 0 ? pdsharp.utils.Random.Element(enemies) : Dummy;

            return Enemy;
        }

        public override string Description()
        {
            return "This illusion bears a close resemblance to you, " + "but it's paler and twitches a little.";
        }

        public override CharSprite Sprite
        {
            get
            {
                var s = base.Sprite;

                ((MirrorSprite)s).UpdateArmor(tier);

                return s;
            }
        }

        public override void Interact()
        {
            var curPos = pos;

            MoveSprite(pos, Dungeon.Hero.pos);
            Move(Dungeon.Hero.pos);

            Dungeon.Hero.Sprite.Move(Dungeon.Hero.pos, curPos);
            Dungeon.Hero.Move(curPos);

            Dungeon.Hero.Spend(1 / Dungeon.Hero.Speed());
            Dungeon.Hero.Busy();
        }

        public override bool Reset()
        {
            return true;
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static MirrorImage()
        {
            IMMUNITIES.Add(typeof(ToxicGas));
            IMMUNITIES.Add(typeof(Burning));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}