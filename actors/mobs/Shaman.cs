using System.Collections.Generic;
using pdsharp.utils;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.levels.traps;
using sharpdungeon.mechanics;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.levels;
using pdsharp.noosa;
using System;

namespace sharpdungeon.actors.mobs
{
    public class Shaman : Mob, ICallback
    {
        private const float TimeToZap = 2f;

        private const string TxtLightningKilled = "{0}'s lightning bolt killed you...";

        public Shaman()
        {
            Name = "gnoll shaman";
            SpriteClass = typeof(ShamanSprite);

            HP = HT = 18;
            defenseSkill = 8;

            Exp = 6;
            MaxLvl = 14;

            loot = Generator.Category.SCROLL;
            lootChance = 0.33f;
        }

        public override int DamageRoll()
        {
            return pdsharp.utils.Random.NormalIntRange(2, 6);
        }

        public override int AttackSkill(Character target)
        {
            return 11;
        }

        public override int Dr()
        {
            return 4;
        }

        protected internal override bool CanAttack(Character enemy)
        {
            return Ballistica.Cast(pos, enemy.pos, false, true) == enemy.pos;
        }

        protected internal override bool DoAttack(Character enemy)
        {
            if (Level.Distance(pos, enemy.pos) <= 1)
                return base.DoAttack(enemy);

            var visible = Level.fieldOfView[pos] || Level.fieldOfView[enemy.pos];
            if (visible)
                ((ShamanSprite)Sprite).DoZap(enemy.pos);

            Spend(TimeToZap);

            if (Hit(this, enemy, true))
            {
                var dmg = (float)pdsharp.utils.Random.Int(2, 12);

                if (Level.water[enemy.pos] && !enemy.Flying)
                    dmg *= 1.5f;

                enemy.Damage((int)dmg, LightningTrap.LIGHTNING);

                enemy.Sprite.CenterEmitter().Burst(SparkParticle.Factory, 3);
                enemy.Sprite.Flash();

                if (enemy != Dungeon.Hero)
                    return !visible;

                Camera.Main.Shake(2, 0.3f);

                if (enemy.IsAlive)
                    return !visible;

                Dungeon.Fail(Utils.Format(ResultDescriptions.MOB, Utils.Indefinite(Name), Dungeon.Depth));
                GLog.Negative(TxtLightningKilled, Name);
            }
            else
                enemy.Sprite.ShowStatus(CharSprite.Neutral, enemy.DefenseVerb());

            return !visible;
        }

        public void Call()
        {
            Next();
        }

        public override string Description()
        {
            return "The most intelligent gnolls can master shamanistic magic. Gnoll shamans prefer " + "battle spells to compensate for lack of might, not hesitating to use them " + "on those who question their status in a tribe.";
        }

        private static readonly HashSet<Type> RESISTANCES = new HashSet<Type>();
        static Shaman()
        {
            RESISTANCES.Add(typeof(LightningTrap.Electricity));
        }

        public override HashSet<Type> Resistances()
        {
            return RESISTANCES;
        }
    }
}