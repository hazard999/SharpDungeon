using System;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.items.armor.glyphs
{
    public class Viscosity : Glyph
    {
        private const string TxtViscosity = "{0} of viscosity";

        private static readonly ItemSprite.Glowing Purple = new ItemSprite.Glowing(0x8844CC);

        public override int Proc(Armor armor, Character attacker, Character defender, int damage)
        {
            if (damage == 0)
                return 0;

            var level = Math.Max(0, armor.level);

            if (pdsharp.utils.Random.Int(level + 7) < 6) 
                return damage;

            var debuff = defender.Buff<DeferedDamage>();
            if (debuff == null)
            {
                debuff = new DeferedDamage();
                debuff.AttachTo(defender);
            }

            debuff.Prolong(damage);

            defender.Sprite.ShowStatus(CharSprite.Warning, "deferred {0}", damage);

            return 0;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtViscosity, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Purple;
        }

        public class DeferedDamage : Buff
        {

            protected internal int damage = 0;

            private const string DAMAGE = "damage";

            public override void StoreInBundle(Bundle bundle)
            {
                base.StoreInBundle(bundle);
                bundle.Put(DAMAGE, damage);
            }

            public override void RestoreFromBundle(Bundle bundle)
            {
                base.RestoreFromBundle(bundle);
                damage = bundle.GetInt(DAMAGE);
            }

            public override bool AttachTo(Character target)
            {
                if (!base.AttachTo(target)) 
                    return false;

                Postpone(Tick);
                return true;
            }

            public virtual void Prolong(int damage)
            {
                this.damage += damage;
            }

            public override int Icon()
            {
                return BuffIndicator.DEFERRED;
            }

            public override string ToString()
            {
                return Utils.Format("Defered damage ({0})", damage);
            }

            protected override bool Act()
            {
                if (Target.IsAlive)
                {
                    Target.Damage(1, this);
                    if (Target == Dungeon.Hero && !Target.IsAlive)
                    {
                        // FIXME
                        var glyph = new Viscosity();
                        Dungeon.Fail(Utils.Format(ResultDescriptions.GLYPH, glyph.Name(), Dungeon.Depth));
                        GLog.Negative("{0} killed you...", glyph.Name());

                        Badge.ValidateDeathFromGlyph();
                    }
                    Spend(Tick);

                    if (--damage <= 0)
                        Detach();
                }
                else
                    Detach();

                return true;
            }
        }
    }
}