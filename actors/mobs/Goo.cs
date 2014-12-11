using System.Collections.Generic;
using Android.Graphics;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using System;
using sharpdungeon.utils;

namespace sharpdungeon.actors.mobs
{
    public class Goo : Mob
    {
        private const float PumpUpDelay = 2f;

        public Goo()
        {
            Name = "Goo";
            HP = HT = 80;
            Exp = 10;
            defenseSkill = 12;
            SpriteClass = typeof(GooSprite);

            loot = new LloydsBeacon();
            lootChance = 0.333f;
        }

        private bool _pumpedUp;

        public override int DamageRoll()
        {
            if (_pumpedUp)
                return pdsharp.utils.Random.NormalIntRange(5, 30);

            return pdsharp.utils.Random.NormalIntRange(2, 12);
        }

        public override int AttackSkill(Character target)
        {
            return _pumpedUp ? 30 : 15;
        }

        public override int Dr()
        {
            return 2;
        }

        protected override bool Act()
        {
            if (!Level.water[pos] || HP >= HT)
                return base.Act();

            Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
            HP++;

            return base.Act();
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return _pumpedUp ? Distance(enemy) <= 2 : base.CanAttack(enemy);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (pdsharp.utils.Random.Int(3) == 0)
            {
                buffs.Buff.Affect<Ooze>(enemy);
                enemy.Sprite.Burst(new Color(0x000000), 5);
            }

            if (_pumpedUp)
                pdsharp.noosa.Camera.Main.Shake(3, 0.2f);

            return damage;
        }

        protected internal override bool DoAttack(Character enemy)
        {
            if (_pumpedUp || pdsharp.utils.Random.Int(3) > 0)
                return base.DoAttack(enemy);

            _pumpedUp = true;
            Spend(PumpUpDelay);

            ((GooSprite)Sprite).PumpUp();

            if (!Dungeon.Visible[pos]) 
                return true;

            Sprite.ShowStatus(CharSprite.Negative, "!!!");
            GLog.Negative("Goo is pumping itself up!");

            return true;
        }

        public override bool Attack(Character enemy)
        {
            bool result = base.Attack(enemy);
            _pumpedUp = false;
            return result;
        }

        protected internal override bool GetCloser(int target)
        {
            _pumpedUp = false;
            return base.GetCloser(target);
        }

        public override void Move(int step)
        {
            ((SewerBossLevel)Dungeon.Level).Seal();
            base.Move(step);
        }

        public override void Die(object cause)
        {
            base.Die(cause);

            ((SewerBossLevel)Dungeon.Level).Unseal();

            GameScene.BossSlain();
            Dungeon.Level.Drop(new SkeletonKey(), pos).Sprite.Drop();

            Badge.ValidateBossSlain();

            Yell("glurp... glurp...");
        }

        public override void Notice()
        {
            base.Notice();
            Yell("GLURP-GLURP!");
        }

        public override string Description()
        {
            return "Little known about The Goo. It's quite possible that it is not even a creature, but rather a " + "conglomerate of substances from the sewers that gained rudiments of free will.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Goo()
        {
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
        }

        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}