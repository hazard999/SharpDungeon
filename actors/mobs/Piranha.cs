using System;
using System.Collections.Generic;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.items.food;
using sharpdungeon.levels;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Piranha : Mob
    {
        public Piranha()
        {
            HP = HT = 10 + Dungeon.Depth * 5;
            defenseSkill = 10 + Dungeon.Depth * 2;
            Name = "giant piranha";
            SpriteClass = typeof(PiranhaSprite);

            baseSpeed = 2f;

            Exp = 0;
        }

        protected override bool Act()
        {
            if (!Level.water[pos])
            {
                Die(null);
                return true;
            }

            return base.Act();
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(Dungeon.Depth, 4 + Dungeon.Depth * 2);
        }

        public override int AttackSkill(Character target)
        {
            return 20 + Dungeon.Depth * 2;
        }

        public override int Dr()
        {
            return Dungeon.Depth;
        }

        public override void Die(object cause)
        {
            Dungeon.Level.Drop(new MysteryMeat(), pos).Sprite.Drop();
            base.Die(cause);

            Statistics.PiranhasKilled++;
            Badge.ValidatePiranhasKilled();
        }

        public override bool Reset()
        {
            return true;
        }

        protected internal override bool GetCloser(int target)
        {
            if (Rooted)
                return false;

            var step = Dungeon.FindPath(this, pos, target, Level.water, Level.fieldOfView);

            if (step == -1)
                return false;

            Move(step);
            return true;
        }

        protected internal override bool GetFurther(int target)
        {
            var step = Dungeon.Flee(this, pos, target, Level.water, Level.fieldOfView);

            if (step == -1) 
                return false;

            Move(step);
            return true;
        }

        public override string Description()
        {
            return "These carnivorous fish are not natural inhabitants of underground pools. " + "They were bred specifically to protect flooded treasure vaults.";
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Piranha()
        {
            IMMUNITIES.Add(typeof(Burning));
            IMMUNITIES.Add(typeof(Paralysis));
            IMMUNITIES.Add(typeof(ToxicGas));
            IMMUNITIES.Add(typeof(Roots));
            IMMUNITIES.Add(typeof(Frost));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}