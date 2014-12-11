using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.items.food;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Spinner : Mob
    {
        public Spinner()
        {
            Name = "cave spinner";
            SpriteClass = typeof(SpinnerSprite);

            HP = HT = 50;
            defenseSkill = 14;

            Exp = 9;
            MaxLvl = 16;

            loot = new MysteryMeat();
            lootChance = 0.125f;

            FLEEING = new SpinnerFleeing(this);
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(12, 16);
        }

        public override int AttackSkill(Character target)
        {
            return 20;
        }

        public override int Dr()
        {
            return 6;
        }

        protected override bool Act()
        {
            var result = base.Act();

            if (State == FLEEING && Buff<Terror>() == null && EnemySeen && Enemy.Buff<Poison>() == null)
                State = HUNTING;

            return result;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(2) != 0)
                return damage;

            buffs.Buff.Affect<Poison>(enemy).Set(Random.Int(7, 9) * Poison.DurationFactor(enemy));
            State = FLEEING;

            return damage;
        }

        public override void Move(int step)
        {
            if (State == FLEEING)
                GameScene.Add(Blob.Seed(pos, Random.Int(5, 7), typeof(Web)));

            base.Move(step);
        }

        public override string Description()
        {
            return "These greenish furry cave spiders try to avoid direct combat, preferring to wait in the distance " + "while their victim, entangled in the spinner's excreted cobweb, slowly dies from their poisonous bite.";
        }

        private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();
        static Spinner()
        {
            RESISTANCES.Add(typeof(Poison));
            IMMUNITIES.Add(typeof(Roots));
        }

        public override HashSet<System.Type> Resistances()
        {
            return RESISTANCES;
        }

        private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();

        public override HashSet<System.Type> Immunities()
        {
            return IMMUNITIES;
        }

        private class SpinnerFleeing : Fleeing
        {
            public SpinnerFleeing(Mob mob)
                : base(mob)
            {
            }

            protected override void NowhereToRun()
            {
                if (Mob.Buff<Terror>() == null)
                    Mob.State = Mob.HUNTING;
                else
                    base.NowhereToRun();
            }
        }
    }
}