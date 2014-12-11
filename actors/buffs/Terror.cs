using pdsharp.utils;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Terror : FlavourBuff
    {
        public const float Duration = 10f;
        public Character Source;

        public override int Icon()
        {
            return BuffIndicator.TERROR;
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            // It's not really correct...
            Source = Dungeon.Hero;
        }

        public override string ToString()
        {
            return "Terror";
        }

        public static void Recover(Character target)
        {
            var terror = target.Buff<Terror>();
            if (terror != null && terror.Cooldown() < Duration)
                target.Remove(terror);
        }
    }
}