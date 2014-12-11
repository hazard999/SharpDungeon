using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.items.food;
using sharpdungeon.items.potions;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.mechanics;
using Poison = sharpdungeon.actors.buffs.Poison;
using System;
using sharpdungeon.sprites;
using sharpdungeon.levels;

namespace sharpdungeon.actors.mobs
{
    public class Scorpio : Mob
    {
        public Scorpio()
        {
            Name = "scorpio";
            SpriteClass = typeof(ScorpioSprite);

            HP = HT = 95;
            defenseSkill = 24;
            viewDistance = Light.Distance;

            Exp = 14;
            MaxLvl = 25;

            loot = new PotionOfHealing();
            lootChance = 0.125f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(20, 32);
        }

        public override int AttackSkill(Character target)
        {
            return 36;
        }

        public override int Dr()
        {
            return 16;
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return !Level.Adjacent(pos, enemy.pos) && Ballistica.Cast(pos, enemy.pos, false, true) == enemy.pos;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (pdsharp.utils.Random.Int(2) == 0)
                buffs.Buff.Prolong<Cripple>(enemy, Cripple.Duration);

            return damage;
        }

        protected internal override bool GetCloser(int target)
        {
            if (State == HUNTING)
                return EnemySeen && GetFurther(target);

            return base.GetCloser(target);
        }

        protected internal override void DropLoot()
        {
            if (pdsharp.utils.Random.Int(8) == 0)
                Dungeon.Level.Drop(new PotionOfHealing(), pos).Sprite.Drop();
            else
                if (pdsharp.utils.Random.Int(6) == 0)
                    Dungeon.Level.Drop(new MysteryMeat(), pos).Sprite.Drop();
        }

        public override string Description()
        {
            return "These huge arachnid-like demonic creatures avoid close combat by All means, " + "firing crippling serrated spikes from long distances.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Scorpio()
        {
            RESISTANCES.Add(typeof(Leech));
            RESISTANCES.Add(typeof(Poison));
        }

        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}