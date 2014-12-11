using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.mechanics;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using System;
using System.Collections.Generic;

namespace sharpdungeon.actors.mobs
{
    public class Eye : Mob
    {
        private const string TxtDeathgazeKilled = "{0}'s deathgaze killed you...";

        public Eye()
        {
            Name = "evil eye";
            SpriteClass = typeof(EyeSprite);

            HP = HT = 100;
            defenseSkill = 20;
            viewDistance = Light.Distance;

            Exp = 13;
            MaxLvl = 25;

            Flying = true;

            loot = new Dewdrop();
            lootChance = 0.5f;
        }

        public override int Dr()
        {
            return 10;
        }

        private int hitCell;

        protected internal override bool CanAttack(Character enemy)
        {
            hitCell = Ballistica.Cast(pos, enemy.pos, true, false);

            for (var i = 1; i < Ballistica.Distance; i++)
                if (Ballistica.Trace[i] == enemy.pos)
                    return true;

            return false;
        }

        public override int AttackSkill(Character target)
        {
            return 30;
        }

        protected internal override float AttackDelay()
        {
            return 1.6f;
        }

        protected internal override bool DoAttack(Character enemy)
        {
            Spend(AttackDelay());

            var rayVisible = false;

            for (var i = 0; i < Ballistica.Distance; i++)
                if (Dungeon.Visible[Ballistica.Trace[i]])
                    rayVisible = true;

            if (rayVisible)
            {
                Sprite.DoAttack(hitCell);
                return false;
            }

            Attack(enemy);
            return true;
        }

        public override bool Attack(Character enemy)
        {
            for (var i = 1; i < Ballistica.Distance; i++)
            {
                var localPos = Ballistica.Trace[i];

                var ch = FindChar(localPos);
                if (ch == null)
                    continue;

                if (Hit(this, ch, true))
                {
                    ch.Damage(pdsharp.utils.Random.NormalIntRange(14, 20), this);

                    if (Dungeon.Visible[localPos])
                    {
                        ch.Sprite.Flash();
                        CellEmitter.Center(localPos).Burst(PurpleParticle.Burst, pdsharp.utils.Random.IntRange(1, 2));
                    }

                    if (ch.IsAlive || ch != Dungeon.Hero)
                        continue;

                    Dungeon.Fail(Utils.Format(ResultDescriptions.MOB, Utils.Indefinite(Name), Dungeon.Depth));
                    GLog.Negative(TxtDeathgazeKilled, Name);
                }
                else
                    ch.Sprite.ShowStatus(CharSprite.Neutral, ch.DefenseVerb());
            }

            return true;
        }

        public override string Description()
        {
            return "One of this demon's other names is \"orb of hatred\", because when it sees an enemy, " + "it uses its deathgaze recklessly, often ignoring its allies and wounding them.";
        }
        
        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Eye()
        {
            RESISTANCES.Add(typeof(WandOfDisintegration));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(Leech));
            IMMUNITIES.Add(typeof(Terror));
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