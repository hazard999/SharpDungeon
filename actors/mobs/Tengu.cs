using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Tengu : Mob
    {
        private const int JumpDelay = 5;

        public Tengu()
        {
            Name = "Tengu";
            SpriteClass = typeof(TenguSprite);

            HP = HT = 120;
            Exp = 20;
            defenseSkill = 20;
        }

        private int _timeToJump = JumpDelay;

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(8, 15);
        }

        public override int AttackSkill(Character target)
        {
            return 20;
        }

        public override int Dr()
        {
            return 5;
        }

        public override void Die(object cause)
        {
            Badge badgeToCheck;
            switch (Dungeon.Hero.heroClass.Ordinal())
            {
                case HeroClassType.Warrior:
                    badgeToCheck = Badge.MASTERY_WARRIOR;
                    break;
                case HeroClassType.Mage:
                    badgeToCheck = Badge.MASTERY_MAGE;
                    break;
                case HeroClassType.Rogue:
                    badgeToCheck = Badge.MASTERY_ROGUE;
                    break;
                case HeroClassType.Huntress:
                    badgeToCheck = Badge.MASTERY_HUNTRESS;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!Badge.IsUnlocked(badgeToCheck))
                Dungeon.Level.Drop(new TomeOfMastery(), pos).Sprite.Drop();

            GameScene.BossSlain();
            Dungeon.Level.Drop(new SkeletonKey(), pos).Sprite.Drop();
            base.Die(cause);

            Badge.ValidateBossSlain();

            Yell("Free at last...");
        }

        protected internal override bool GetCloser(int target)
        {
            if (Level.fieldOfView[target])
            {
                Jump();
                return true;
            }
            
            return base.GetCloser(target);
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return Ballistica.Cast(pos, enemy.pos, false, true) == enemy.pos;
        }

        protected internal override bool DoAttack(Character enemy)
        {
            _timeToJump--;
            
            if (_timeToJump > 0 || !Level.Adjacent(pos, enemy.pos)) 
                return base.DoAttack(enemy);

            Jump();
            return true;
        }

        private void Jump()
        {
            _timeToJump = JumpDelay;

            for (var i = 0; i < 4; i++)
            {
                int trapPos;
                
                do
                {
                    trapPos = pdsharp.utils.Random.Int(Level.Length);
                } 
                while (!Level.fieldOfView[trapPos] || !Level.passable[trapPos]);

                if (Dungeon.Level.map[trapPos] != Terrain.INACTIVE_TRAP) 
                    continue;

                Level.Set(trapPos, Terrain.POISON_TRAP);
                GameScene.UpdateMap(trapPos);
                ScrollOfMagicMapping.Discover(trapPos);
            }

            int newPos;
            do
            {
                newPos = pdsharp.utils.Random.Int(Level.Length);
            } 
            while (!Level.fieldOfView[newPos] || !Level.passable[newPos] || Level.Adjacent(newPos, Enemy.pos) || FindChar(newPos) != null);

            Sprite.Move(pos, newPos);
            Move(newPos);

            if (Dungeon.Visible[newPos])
            {
                CellEmitter.Get(newPos).Burst(Speck.Factory(Speck.WOOL), 6);
                Sample.Instance.Play(Assets.SND_PUFF);
            }

            Spend(1 / Speed());
        }

        public override void Notice()
        {
            base.Notice();
            Yell("Gotcha, " + Dungeon.Hero.heroClass.Title() + "!");
        }

        public override string Description()
        {
            return "Tengu are members of the ancient assassins clan, which is also called Tengu. " + "These assassins are noted for extensive use of shuriken and traps.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Tengu()
        {
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Poison));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
        }

        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}