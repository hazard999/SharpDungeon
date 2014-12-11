using System.Collections.Generic;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.actors.mobs
{
    public class Bat : Mob
    {
        public Bat()
        {
            Name = "vampire bat";
            SpriteClass = typeof(BatSprite);

            HP = HT = 30;
            defenseSkill = 15;
            baseSpeed = 2f;

            Exp = 7;
            MaxLvl = 15;

            Flying = true;

            loot = new PotionOfHealing();
            lootChance = 0.125f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(6, 12);
        }

        public override int AttackSkill(Character target)
        {
            return 16;
        }

        public override int Dr()
        {
            return 4;
        }

        public override string DefenseVerb()
        {
            return "evaded";
        }

        public override int AttackProc(Character enemy, int damage)
        {
            var reg = Math.Min(damage, HT - HP);

            if (reg <= 0) 
                return damage;

            HP += reg;
            Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);

            return damage;
        }

        public override string Description()
        {
            return "These brisk and tenacious inhabitants of cave domes may defeat much larger opponents by " + "replenishing their health with each successful Attack.";
        }
        
        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Bat()
        {
            RESISTANCES.Add(typeof(Leech));
        }
        
        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}