using System;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Buff : Actor
    {
        public Character Target;

        public virtual bool AttachTo(Character target)
        {
            if (target.Immunities().Contains(GetType()))
                return false;

            Target = target;
            Add(this);

            return true;
        }

        public virtual void Detach()
        {
            Target.Remove(this);
        }

        protected override bool Act()
        {
            Deactivate();
            return true;
        }

        public virtual int Icon()
        {
            return BuffIndicator.NONE;
        }

        public static T Affect<T>(Character target) where T : Buff, new()
        {
            var buff = target.Buff<T>();

            if (buff != null)
                return buff;

            try
            {
                buff = new T();
                buff.AttachTo(target);
                return buff;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T Affect<T>(Character target, float duration) where T : Buff, new()
        {
            var buff = Affect<T>(target);

            buff.Spend(duration);

            return buff;
        }

        public static T Prolong<T>(Character target, float duration) where T : Buff, new()
        {
            var buff = Affect<T>(target);

            buff.Postpone(duration);

            return buff;
        }

        public static void Detach(Buff buff)
        {
            if (buff != null)
                buff.Detach();
        }

        public static void Detach<T>(Character target) where T : Buff
        {
            Detach(target.Buff<T>());
        }

        public static void Detach(Character target, Buff cl)
        {
            Detach(target.Buff(cl));
        }
    }
}