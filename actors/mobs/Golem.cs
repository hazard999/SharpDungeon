using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.scrolls;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.actors.mobs
{
    public class Golem : Mob
    {
        public Golem()
        {
            Name = "golem";
            SpriteClass = typeof(GolemSprite);

            HP = HT = 85;
            defenseSkill = 18;

            Exp = 12;
            MaxLvl = 22;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(20, 40);
        }

        public override int AttackSkill(Character target)
        {
            return 28;
        }

        protected internal override float AttackDelay()
        {
            return 1.5f;
        }

        public override int Dr()
        {
            return 12;
        }

        public override string DefenseVerb()
        {
            return "blocked";
        }

        public override void Die(object cause)
        {
            Imp.Quest.Process(this);

            base.Die(cause);
        }

        public override string Description()
        {
            return "The Dwarves tried to combine their knowledge of mechanisms with their newfound power of elemental binding. " + "They used spirits of earth as the \"soul\" for the mechanical bodies of golems, which were believed to be " + "most controllable of All. Despite this, the tiniest mistake in the ritual could cause an outbreak.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Golem()
        {
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            IMMUNITIES.Add(typeof(Amok));
            IMMUNITIES.Add(typeof(Terror));
            IMMUNITIES.Add(typeof(Sleep));
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