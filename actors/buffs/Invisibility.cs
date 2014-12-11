using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Invisibility : FlavourBuff
    {
        public const float Duration = 15f;

        public override bool AttachTo(Character target)
        {
            if (!base.AttachTo(target))
                return false;

            target.invisible++;
            return true;
        }

        public override void Detach()
        {
            Target.invisible--;
            base.Detach();
        }

        public override int Icon()
        {
            return BuffIndicator.INVISIBLE;
        }

        public override string ToString()
        {
            return "Invisible";
        }

        public static void Dispel()
        {
            var buff = Dungeon.Hero.Buff<Invisibility>();
            if (buff != null && Dungeon.Hero.VisibleEnemies > 0)
                buff.Detach();
        }
    }
}