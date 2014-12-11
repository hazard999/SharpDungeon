using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Roots : FlavourBuff
    {
        public override bool AttachTo(Character target)
        {
            if (!target.Flying && base.AttachTo(target))
            {
                target.Rooted = true;
                return true;
            }

            return false;
        }

        public override void Detach()
        {
            Target.Rooted = false;
            base.Detach();
        }

        public override int Icon()
        {
            return BuffIndicator.ROOTS;
        }

        public override string ToString()
        {
            return "Rooted";
        }
    }
}