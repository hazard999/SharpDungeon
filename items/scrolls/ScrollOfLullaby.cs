using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfLullaby : Scroll
    {
        public ScrollOfLullaby()
        {
            name = "Scroll of Lullaby";
        }

        protected internal override void DoRead()
        {
            CurUser.Sprite.CenterEmitter().Start(Speck.Factory(Speck.NOTE), 0.3f, 5);
            Sample.Instance.Play(Assets.SND_LULLABY);
            Invisibility.Dispel();

            var count = 0;
            Mob affected = null;
            foreach (var mob in Dungeon.Level.mobs)
            {
                if (!Level.fieldOfView[mob.pos])
                    continue;

                Buff.Affect<Sleep>(mob);

                if (mob.Buff<Sleep>() == null)
                    continue;

                affected = mob;
                count++;
            }

            switch (count)
            {
                case 0:
                    GLog.Information("The scroll utters a soothing melody.");
                    break;
                case 1:
                    if (affected != null)
                        GLog.Information("The scroll utters a soothing melody and the " + affected.Name + " falls asleep!");
                    else
                        GLog.Information("The scroll utters a soothing melody.");
                    break;
                default:
                    GLog.Information("The scroll utters a soothing melody and the monsters fall asleep!");
                    break;
            }
            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "A soothing melody will put All creatures in your field of view into a deep sleep, " + "giving you a chance to flee or make a surprise DoAttack on them.";
        }

        public override int Price()
        {
            return IsKnown ? 50 * quantity : base.Price();
        }
    }
}