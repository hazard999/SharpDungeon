using System.Linq;
using Android.Graphics;
using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfPsionicBlast : Scroll
    {
        public ScrollOfPsionicBlast()
        {
            name = "Scroll of Psionic Blast";
        }

        protected internal override void DoRead()
        {

            GameScene.Flash(new Color(0xFFFFFF));

            Sample.Instance.Play(Assets.SND_BLAST);
            Invisibility.Dispel();

            foreach (var mob in Dungeon.Level.mobs.Where(mob => Level.fieldOfView[mob.pos]))
            {
                Buff.Prolong<Blindness>(mob, pdsharp.utils.Random.Int(3, 6));
                mob.Damage(pdsharp.utils.Random.IntRange(1, mob.HT * 2 / 3), this);
            }

            Buff.Prolong<Blindness>(CurUser, pdsharp.utils.Random.Int(3, 6));
            Dungeon.Observe();

            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "This scroll contains destructive energy, that can be psionically channeled to inflict a " +
                "massive damage to All creatures within a field of view. An accompanying flash of light will " +
                "temporarily blind everybody in the area of effect including the reader of the scroll.";
        }

        public override int Price()
        {
            return IsKnown ? 80 * Quantity() : base.Price();
        }
    }
}