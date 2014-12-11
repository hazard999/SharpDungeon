using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.items;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.utils;
using sharpdungeon.sprites;
using sharpdungeon.levels;
using System;

namespace sharpdungeon.actors.mobs
{
    public class Skeleton : Mob
    {
        private const string TxtHeroKilled = "You were killed by the explosion of bones...";

        public Skeleton()
        {
            Name = "skeleton";
            SpriteClass = typeof(SkeletonSprite);

            HP = HT = 25;
            defenseSkill = 9;

            Exp = 5;
            MaxLvl = 10;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(3, 8);
        }

        public override void Die(object cause)
        {
            base.Die(cause);

            var heroKilled = false;
            for (var i = 0; i < Level.NEIGHBOURS8.Length; i++)
            {
                var ch = FindChar(pos + Level.NEIGHBOURS8[i]);
                
                if (ch == null || !ch.IsAlive) 
                    continue;

                var damage = Math.Max(0, DamageRoll() - pdsharp.utils.Random.IntRange(0, ch.Dr() / 2));
                ch.Damage(damage, this);
                if (ch == Dungeon.Hero && !ch.IsAlive)
                    heroKilled = true;
            }

            if (Dungeon.Visible[pos])
                Sample.Instance.Play(Assets.SND_BONES);

            if (!heroKilled) 
                return;

            Dungeon.Fail(Utils.Format(ResultDescriptions.MOB, Utils.Indefinite(Name), Dungeon.Depth));
            GLog.Negative(TxtHeroKilled);
        }

        protected internal override void DropLoot()
        {
            if (pdsharp.utils.Random.Int(5) != 0) 
                return;

            var localLoot = Generator.Random(Generator.Category.WEAPON);
            for (var i = 0; i < 2; i++)
            {
                var l = Generator.Random(Generator.Category.WEAPON);
                
                if (l.level < localLoot.level)
                    localLoot = l;
            }

            Dungeon.Level.Drop(localLoot, pos).Sprite.Drop();
        }

        public override int AttackSkill(Character target)
        {
            return 12;
        }

        public override int Dr()
        {
            return 5;
        }

        public override string DefenseVerb()
        {
            return "blocked";
        }

        public override string Description()
        {
            return "Skeletons are composed of corpses bones from unlucky adventurers and inhabitants of the dungeon, " + "animated by emanations of evil magic from the depths below. After they have been " + "damaged enough, they disintegrate in an explosion of bones.";
        }

        private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();
        static Skeleton()
        {
            IMMUNITIES.Add(typeof(Death));
        }

        public override HashSet<Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}