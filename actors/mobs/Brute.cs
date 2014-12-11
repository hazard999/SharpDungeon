using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.items;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using pdsharp.utils;
using System;

namespace sharpdungeon.actors.mobs
{
    public class Brute : Mob
    {
        private const string TxtEnraged = "{0} becomes enraged!";

        public Brute()
        {
            Name = "gnoll brute";
            SpriteClass = typeof(BruteSprite);

            HP = HT = 40;
            defenseSkill = 15;

            Exp = 8;
            MaxLvl = 15;

            loot = new Gold();
            lootChance = 0.5f;
        }

        private bool _enraged;

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _enraged = HP < HT / 4;
        }

        public override int DamageRoll()
        {
            return _enraged ? pdsharp.utils.Random.NormalIntRange(10, 40) : pdsharp.utils.Random.NormalIntRange(8, 18);
        }

        public override int AttackSkill(Character target)
        {
            return 20;
        }

        public override int Dr()
        {
            return 8;
        }

        public override void Damage(int dmg, object src)
        {
            base.Damage(dmg, src);

            if (!IsAlive || _enraged || HP >= HT / 4)
                return;

            _enraged = true;
            Spend(Tick);

            if (!Dungeon.Visible[pos])
                return;

            GLog.Warning(TxtEnraged, Name);
            Sprite.ShowStatus(CharSprite.Negative, "enraged");
        }

        public override string Description()
        {
            return "Brutes are the largest, strongest and toughest of All gnolls. When severely wounded, " + "they go berserk, inflicting even more damage to their enemies.";
        }
        
        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Brute()
        {
            IMMUNITIES.Add(typeof(Terror));
        }
        
        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}