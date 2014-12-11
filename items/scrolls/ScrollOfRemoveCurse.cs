using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfRemoveCurse : Scroll
    {
        private const string TxtProcced = "Your pack glows with a cleansing light, and a malevolent energy disperses.";
        private const string TxtNotProcced = "Your pack glows with a cleansing light, but nothing happens.";

        public ScrollOfRemoveCurse()
        {
            name = "Scroll of Remove Curse";
        }

        protected internal override void DoRead()
        {
            new Flare(6, 32).Show(CurUser.Sprite, 2f);
            Sample.Instance.Play(Assets.SND_READ);
            Invisibility.Dispel();

            var procced = Uncurse(CurUser, CurUser.Belongings.Backpack.Items.ToArray());
            procced = Uncurse(CurUser, CurUser.Belongings.Weapon, CurUser.Belongings.Armor, CurUser.Belongings.Ring1, CurUser.Belongings.Ring2) || procced;

            Buff.Detach<Weakness>(CurUser);

            if (procced)
                GLog.Positive(TxtProcced);
            else
                GLog.Information(TxtNotProcced);

            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "The incantation on this scroll will instantly strip from " + "the reader's weapon, armor, rings and carried items any evil " + "enchantments that might prevent the wearer from removing them.";
        }

        public static bool Uncurse(Hero hero, params Item[] items)
        {
            var procced = false;
            foreach (var item in items)
            {
                if (item == null || !item.cursed)
                    continue;

                item.cursed = false;
                procced = true;
            }

            if (procced)
                hero.Sprite.Emitter().Start(ShadowParticle.Up, 0.05f, 10);

            return procced;
        }

        public override int Price()
        {
            return IsKnown ? 30 * quantity : base.Price();
        }
    }
}