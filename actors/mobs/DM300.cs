using System.Collections.Generic;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.keys;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using System;
using Paralysis = sharpdungeon.actors.buffs.Paralysis;

namespace sharpdungeon.actors.mobs
{
    public class DM300 : Mob
    {
        public DM300()
        {
            Name = "DM-300";
            SpriteClass = typeof(DM300Sprite);

            HP = HT = 200;
            Exp = 30;
            defenseSkill = 18;

            loot = new RingOfThorns().Random();
            lootChance = 0.333f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(18, 24);
        }

        public override int AttackSkill(Character target)
        {
            return 28;
        }

        public override int Dr()
        {
            return 10;
        }

        protected override bool Act()
        {
            GameScene.Add(Blob.Seed(pos, 30, typeof(ToxicGas)));

            return base.Act();
        }

        public override void Move(int step)
        {
            base.Move(step);

            if (Dungeon.Level.map[step] == Terrain.INACTIVE_TRAP && HP < HT)
            {
                HP += pdsharp.utils.Random.Int(1, HT - HP);
                Sprite.Emitter().Burst(ElmoParticle.Factory, 5);

                if (Dungeon.Visible[step] && Dungeon.Hero.IsAlive)
                    GLog.Negative("DM-300 repairs itself!");
            }

            int[] cells = { step - 1, step + 1, step - Level.Width, step + Level.Width, step - 1 - Level.Width, step - 1 + Level.Width, step + 1 - Level.Width, step + 1 + Level.Width };
            var cell = cells[pdsharp.utils.Random.Int(cells.Length)];

            if (Dungeon.Visible[cell])
            {
                CellEmitter.Get(cell).Start(Speck.Factory(Speck.ROCK), 0.07f, 10);
                Camera.Main.Shake(3, 0.7f);
                Sample.Instance.Play(Assets.SND_ROCKS);

                if (Level.water[cell])
                    GameScene.Ripple(cell);
                else
                    if (Dungeon.Level.map[cell] == Terrain.EMPTY)
                    {
                        Level.Set(cell, Terrain.EMPTY_DECO);
                        GameScene.UpdateMap(cell);
                    }
            }

            var ch = FindChar(cell);
            if (ch != null && ch != this)
                buffs.Buff.Prolong<Paralysis>(ch, 2);
        }

        public override void Die(object cause)
        {
            base.Die(cause);

            GameScene.BossSlain();
            Dungeon.Level.Drop(new SkeletonKey(), pos).Sprite.Drop();

            Badge.ValidateBossSlain();

            Yell("Mission failed. Shutting down.");
        }

        public override void Notice()
        {
            base.Notice();
            Yell("Unauthorised personnel detected.");
        }

        public override string Description()
        {
            return "This machine was created by the Dwarves several centuries ago. Later, Dwarves started to replace machines with " + "golems, elementals and even demons. Eventually it led their civilization to the decline. The DM-300 and similar " + "machines were typically used for construction and mining, and in some cases, for city defense.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static DM300()
        {
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            IMMUNITIES.Add(typeof(ToxicGas));
        }

        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}