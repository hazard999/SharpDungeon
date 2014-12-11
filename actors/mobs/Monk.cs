using System;
using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.food;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.actors.mobs
{
    public class Monk : Mob
    {
        public const string TxtDisarm = "{0} has knocked the {1} from your hands!";

        public Monk()
        {
            Name = "dwarf monk";
            SpriteClass = typeof(MonkSprite);

            HP = HT = 70;
            defenseSkill = 30;

            Exp = 11;
            MaxLvl = 21;

            loot = new Food();
            lootChance = 0.083f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(12, 16);
        }

        public override int AttackSkill(Character target)
        {
            return 30;
        }

        protected internal override float AttackDelay()
        {
            return 0.5f;
        }

        public override int Dr()
        {
            return 2;
        }

        public override string DefenseVerb()
        {
            return "parried";
        }

        public override void Die(object cause)
        {
            Imp.Quest.Process(this);

            base.Die(cause);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(6) != 0 || enemy != Dungeon.Hero)
                return damage;

            var hero = Dungeon.Hero;
            var weapon = hero.Belongings.Weapon;

            if (weapon == null || weapon is Knuckles || weapon.cursed)
                return damage;

            hero.Belongings.Weapon = null;
            Dungeon.Level.Drop(weapon, hero.pos).Sprite.Drop();
            GLog.Warning(TxtDisarm, Name, weapon.Name);

            return damage;
        }

        public override string Description()
        {
            return "These monks are fanatics, who devoted themselves to protecting their city's secrets from All aliens. " + "They don't use any armor or weapons, relying solely on the art of hand-to-hand combat.";
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Monk()
        {
            IMMUNITIES.Add(typeof(Amok));
            IMMUNITIES.Add(typeof(Terror));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}