using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.items;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.mechanics;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.levels;

namespace sharpdungeon.actors.mobs
{
    public class Warlock : Mob, ICallback
    {
        private const float TimeToZap = 1f;

        private const string TxtShadowboltKilled = "{0}'s shadow bolt killed you...";

        public Warlock()
        {
            Name = "dwarf warlock";
            SpriteClass = typeof(WarlockSprite);

            HP = HT = 70;
            defenseSkill = 18;

            Exp = 11;
            MaxLvl = 21;

            loot = Generator.Category.POTION;
            lootChance = 0.83f;

        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(12, 20);
        }

        public override int AttackSkill(Character target)
        {
            return 25;
        }

        public override int Dr()
        {
            return 8;
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return Ballistica.Cast(pos, enemy.pos, false, true) == enemy.pos;
        }

        public override bool Attack(Character enemy)
        {
            if (Level.Adjacent(pos, enemy.pos))
                return base.Attack(enemy);

            var visible = Level.fieldOfView[pos] || Level.fieldOfView[enemy.pos];
            if (visible)
                ((WarlockSprite)Sprite).Zap(enemy.pos);
            else
                Zap();

            return !visible;
        }

        private void Zap()
        {
            Spend(TimeToZap);

            if (Hit(this, Enemy, true))
            {
                if (Enemy == Dungeon.Hero && Random.Int(2) == 0)
                    buffs.Buff.Prolong<Weakness>(Enemy, Weakness.Duration(Enemy));

                var dmg = Random.Int(12, 18);
                Enemy.Damage(dmg, this);

                if (Enemy.IsAlive || Enemy != Dungeon.Hero)
                    return;

                Dungeon.Fail(Utils.Format(ResultDescriptions.MOB, Utils.Indefinite(Name), Dungeon.Depth));
                GLog.Negative(TxtShadowboltKilled, Name);
            }
            else
                Enemy.Sprite.ShowStatus(CharSprite.Neutral, Enemy.DefenseVerb());
        }

        public virtual void OnZapComplete()
        {
            Zap();
            Next();
        }

        public void Call()
        {
            Next();
        }

        public override string Description()
        {
            return "When dwarves' interests have shifted from engineering to arcane arts, " + "warlocks have come to power in the city. They started with elemental magic, " + "but soon switched to demonology and necromancy.";
        }

        private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();
        static Warlock()
        {
            RESISTANCES.Add(typeof(Death));
        }

        public override HashSet<System.Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}