using System.Linq;
using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfTerror : Scroll
    {
        public ScrollOfTerror()
        {
            name = "Scroll of Terror";
        }

        protected internal override void DoRead()
        {
            new Flare(5, 32).Color(0xFF0000, true).Show(CurUser.Sprite, 2f);
            Sample.Instance.Play(Assets.SND_READ);
            Invisibility.Dispel();

            var count = 0;
            Mob affected = null;
            foreach (var mob in Dungeon.Level.mobs.ToArray())
            {
                if (!Level.fieldOfView[mob.pos]) 
                    continue;

                var terror = Buff.Affect<Terror>(mob, Terror.Duration);
                terror.Source = CurUser;

                count++;
                affected = mob;
            }

            switch (count)
            {
                case 0:
                    GLog.Information("The scroll emits a brilliant flash of red light");
                    break;
                case 1:
                    if (affected != null) 
                        GLog.Information("The scroll emits a brilliant flash of red light and the " + affected.Name + " flees!");
                    else
                        GLog.Information("The scroll emits a brilliant flash of red light");
                    break;
                default:
                    GLog.Information("The scroll emits a brilliant flash of red light and the monsters flee!");
                    break;
            }
            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "A flash of red light will overwhelm All creatures in your field of view with terror, " + "and they will turn and flee. Attacking a fleeing enemy will dispel the effect.";
        }

        public override int Price()
        {
            return IsKnown ? 50 * quantity : base.Price();
        }
    }
}