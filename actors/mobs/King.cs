using System.Linq;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using System.Collections.Generic;
using System;
using sharpdungeon.actors.blobs;
using sharpdungeon.items.scrolls;
using sharpdungeon.sprites;
using sharpdungeon.actors.buffs;
using sharpdungeon.items.weapon.enchantments;

namespace sharpdungeon.actors.mobs
{
    public class King : Mob
    {
        private const int MaxArmySize = 5;

        public King()
        {
            Name = "King of Dwarves";
            SpriteClass = typeof(KingSprite);

            HP = HT = 300;
            Exp = 40;
            defenseSkill = 25;

            Undead.count = 0;
        }

        private bool _nextPedestal = true;

        private const string Pedestal = "pedestal";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Pedestal, _nextPedestal);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _nextPedestal = bundle.GetBoolean(Pedestal);
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(20, 38);
        }

        public override int AttackSkill(Character target)
        {
            return 32;
        }

        public override int Dr()
        {
            return 14;
        }

        public override string DefenseVerb()
        {
            return "parried";
        }

        protected internal override bool GetCloser(int target)
        {
            return CanTryToSummon() ? base.GetCloser(CityBossLevel.Pedestal(_nextPedestal)) : base.GetCloser(target);
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return CanTryToSummon() ? pos == CityBossLevel.Pedestal(_nextPedestal) : Level.Adjacent(pos, enemy.pos);
        }

        private bool CanTryToSummon()
        {
            if (Undead.count < maxArmySize())
            {
                var ch = FindChar(CityBossLevel.Pedestal(_nextPedestal));
                return ch == this || ch == null;
            }

            return false;
        }

        public override bool Attack(Character enemy)
        {
            if (CanTryToSummon() && pos == CityBossLevel.Pedestal(_nextPedestal))
            {
                Summon();
                return true;
            }

            if (FindChar(CityBossLevel.Pedestal(_nextPedestal)) == enemy)
                _nextPedestal = !_nextPedestal;

            return base.Attack(enemy);
        }

        public override void Die(object cause)
        {
            GameScene.BossSlain();
            Dungeon.Level.Drop(new ArmorKit(), pos).Sprite.Drop();
            Dungeon.Level.Drop(new SkeletonKey(), pos).Sprite.Drop();

            base.Die(cause);

            Badge.ValidateBossSlain();

            Yell("You cannot kill me, " + Dungeon.Hero.heroClass.Title() + "... I am... immortal...");
        }

        private int maxArmySize()
        {
            return 1 + MaxArmySize * (HT - HP) / HT;
        }

        private void Summon()
        {
            _nextPedestal = !_nextPedestal;

            Sprite.CenterEmitter().Start(Speck.Factory(Speck.SCREAM), 0.4f, 2);
            Sample.Instance.Play(Assets.SND_CHALLENGE);

            var passable = (bool[])Level.passable.Clone();

            foreach (var actor in All.OfType<Character>())
                passable[(actor).pos] = false;

            var undeadsToSummon = maxArmySize() - Undead.count;

            PathFinder.BuildDistanceMap(pos, passable, undeadsToSummon);
            PathFinder.Distance[pos] = int.MaxValue;
            var dist = 1;

        undeadLabel:
            for (var i = 0; i < undeadsToSummon; i++)
            {
                do
                {
                    for (var j = 0; j < Level.Length; j++)
                    {
                        if (PathFinder.Distance[j] != dist)
                            continue;

                        var undead = new Undead();
                        undead.pos = j;
                        GameScene.Add(undead);

                        WandOfBlink.Appear(undead, j);
                        new Flare(3, 32).Color(0x000000, false).Show(undead.Sprite, 2f);

                        PathFinder.Distance[j] = int.MaxValue;

                        goto undeadLabel;
                    }

                    dist++;
                }
                while (dist < undeadsToSummon);
            }

            Yell("Arise, slaves!");
        }

        public override void Notice()
        {
            base.Notice();
            Yell("How dare you!");
        }

        public override string Description()
        {
            return "The last king of dwarves was known for his deep understanding of processes of life and death. " + "He has persuaded members of his court to participate in a ritual, that should have granted them " + "eternal youthfulness. In the end he was the only one, who got it - and an army of undead " + "as a bonus.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static King()
        {
            RESISTANCES.Add(typeof(ToxicGas));
            RESISTANCES.Add(typeof(Death));
            RESISTANCES.Add(typeof(ScrollOfPsionicBlast));
            RESISTANCES.Add(typeof(WandOfDisintegration));
            IMMUNITIES.Add(typeof(buffs.Paralysis));
            IMMUNITIES.Add(typeof(Death));
            IMMUNITIES.Add(typeof(items.weapon.enchantments.Paralysis));
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

        public class Undead : Mob
        {
            public static int count = 0;

            public Undead()
            {
                Name = "undead dwarf";
                SpriteClass = typeof(UndeadSprite);

                HP = HT = 28;
                defenseSkill = 15;

                Exp = 0;

                State = WANDERING;
            }

            protected internal override void OnAdd()
            {
                count++;
                base.OnAdd();
            }

            protected internal override void OnRemove()
            {
                count--;
                base.OnRemove();
            }

            public override int DamageRoll()
            {
                return pdsharp.utils.Random.NormalIntRange(12, 16);
            }

            public override int AttackSkill(Character target)
            {
                return 16;
            }

            public override int AttackProc(Character enemy, int damage)
            {
                if (pdsharp.utils.Random.Int(MaxArmySize) == 0)
                    buffs.Buff.Prolong<buffs.Paralysis>(enemy, 1);

                return damage;
            }

            public override void Damage(int dmg, object src)
            {
                base.Damage(dmg, src);

                var gas = src as ToxicGas;
                if (gas != null)
                    gas.Clear(pos);
            }

            public override void Die(object cause)
            {
                base.Die(cause);

                if (Dungeon.Visible[pos])
                    Sample.Instance.Play(Assets.SND_BONES);
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
                return "These undead dwarves, risen by the will of the King of Dwarves, were members of his court. " + "They appear as skeletons with a stunning amount of facial hair.";
            }

            private static readonly HashSet<Type> IMMUNITIES = new HashSet<Type>();

            public override HashSet<Type> Immunities()
            {
                return IMMUNITIES;
            }
        }
    }
}