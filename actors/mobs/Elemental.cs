using System;
using System.Collections.Generic;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.sprites;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.actors.mobs
{
    public class Elemental : Mob
    {
        public Elemental()
        {
            Name = "fire elemental";
            SpriteClass = typeof(ElementalSprite);

            HP = HT = 65;
            defenseSkill = 20;

            Exp = 10;
            MaxLvl = 20;

            Flying = true;

            loot = new PotionOfLiquidFlame();
            lootChance = 0.1f;
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(16, 20);
        }

        public override int AttackSkill(Character target)
        {
            return 25;
        }

        public override int Dr()
        {
            return 5;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(2) == 0)
                buffs.Buff.Affect<Burning>(enemy).Reignite(enemy);

            return damage;
        }

        public override void Add(Buff buff)
        {
            if (buff is Burning)
            {
                if (HP >= HT)
                    return;

                HP++;
                Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
            }
            else
            {
                if (buff is Frost)
                    Damage(Random.NormalIntRange(1, HT * 2 / 3), buff);

                Actor.Add(buff);
            }
        }

        public override string Description()
        {
            return "Wandering fire elementals are a byproduct of summoning greater entities. " + "They are too chaotic in their nature to be controlled by even the most powerful demonologist.";
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Elemental()
        {
            IMMUNITIES.Add(typeof(Burning));
            IMMUNITIES.Add(typeof(Fire));
            IMMUNITIES.Add(typeof(WandOfFirebolt));
            IMMUNITIES.Add(typeof(ScrollOfPsionicBlast));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}