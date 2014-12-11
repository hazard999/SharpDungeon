using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Shadows : Invisibility
    {
        protected internal float left;

        private const string Left = "left";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Left, left);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            left = bundle.GetFloat(Left);
        }

        public override bool AttachTo(Character target)
        {
            if (base.AttachTo(target))
            {
                Sample.Instance.Play(Assets.SND_MELD);
                Dungeon.Observe();
                return true;
            }

            return false;
        }

        public override void Detach()
        {
            base.Detach();
            Dungeon.Observe();
        }

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                Spend(Tick * 2);

                if (--left <= 0 || Dungeon.Hero.VisibleEnemies > 0)
                    Detach();
            }
            else
                Detach();

            return true;
        }

        public virtual void Prolong()
        {
            left = 2;
        }

        public override int Icon()
        {
            return BuffIndicator.SHADOWS;
        }

        public override string ToString()
        {
            return "Shadowmelded";
        }
    }
}