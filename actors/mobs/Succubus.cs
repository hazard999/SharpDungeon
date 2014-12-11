using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon.enchantments;
using sharpdungeon.mechanics;
using sharpdungeon.sprites;
using sharpdungeon.levels;

namespace sharpdungeon.actors.mobs
{
    public class Succubus : Mob
    {
        private const int BlinkDelay = 5;

        private int _delay;

        public Succubus()
        {
            Name = "succubus";
            SpriteClass = typeof(SuccubusSprite);

            HP = HT = 80;
            defenseSkill = 25;
            viewDistance = Light.Distance;

            Exp = 12;
            MaxLvl = 25;

            loot = new ScrollOfLullaby();
            lootChance = 0.05f;
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(15, 25);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(3) != 0)
                return damage;

            buffs.Buff.Affect<Charm>(enemy, Charm.durationFactor(enemy) * Random.IntRange(2, 5));
            enemy.Sprite.CenterEmitter().Start(Speck.Factory(Speck.HEART), 0.2f, 5);
            Sample.Instance.Play(Assets.SND_CHARMS);

            return damage;
        }

        protected internal override bool GetCloser(int target)
        {
            if (Level.fieldOfView[target] && Level.Distance(pos, target) > 2 && _delay <= 0)
            {
                Blink(target);
                Spend(-1 / Speed());
                return true;
            }

            _delay--;
            return base.GetCloser(target);
        }

        private void Blink(int target)
        {
            var cell = Ballistica.Cast(pos, target, true, true);

            if (FindChar(cell) != null && Ballistica.Distance > 1)
                cell = Ballistica.Trace[Ballistica.Distance - 2];

            WandOfBlink.Appear(this, cell);

            _delay = BlinkDelay;
        }

        public override int AttackSkill(Character target)
        {
            return 40;
        }

        public override int Dr()
        {
            return 10;
        }

        public override string Description()
        {
            return "The succubi are demons that look like seductive (in a slightly gothic way) girls. Using its magic, the succubus " + "can charm a hero, who will become unable to DoAttack anything until the charm wears off.";
        }
        
        private static readonly HashSet<System.Type> RESISTANCES = new HashSet<System.Type>();
        static Succubus()
        {
            RESISTANCES.Add(typeof(Leech));
            IMMUNITIES.Add(typeof(Sleep));
        }

        public override HashSet<System.Type> Resistances()
        {
            return RESISTANCES;
        }

        private static readonly HashSet<System.Type> IMMUNITIES = new HashSet<System.Type>();
        public override HashSet<System.Type> Immunities()
        {
            return IMMUNITIES;
        }
    }
}