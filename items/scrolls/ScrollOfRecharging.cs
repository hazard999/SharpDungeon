using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfRecharging : Scroll
    {
        public ScrollOfRecharging()
        {
            name = "Scroll of Recharging";
        }

        protected internal override void DoRead()
        {
            var count = CurUser.Belongings.Charge(true);
            Charge(CurUser);

            Sample.Instance.Play(Assets.SND_READ);
            Invisibility.Dispel();

            if (count > 0)
            {
                GLog.Information("a surge of energy courses through your pack, recharging your wand" + (count > 1 ? "s" : ""));
                SpellSprite.Show(CurUser, SpellSprite.Charge);
            }
            else
                GLog.Information("a surge of energy courses through your pack, but nothing happens");

            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "The raw magical power bound up in this parchment will, when released, " + "recharge All of the reader's wands to full power.";
        }

        public static void Charge(Hero hero)
        {
            hero.Sprite.CenterEmitter().Burst(EnergyParticle.Factory, 15);
        }

        public override int Price()
        {
            return IsKnown ? 40 * quantity : base.Price();
        }
    }
}