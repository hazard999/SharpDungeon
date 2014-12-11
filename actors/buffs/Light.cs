using System;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Light : FlavourBuff
    {
        public const float Duration = 250f;
        public const int Distance = 4;

        public override bool AttachTo(Character target)
        {
            if (!base.AttachTo(target)) 
                return false;

            // When a Level is loading, do nothing
            if (Dungeon.Level == null) 
                return true;

            target.viewDistance = Math.Max(Dungeon.Level.viewDistance, Distance);
            Dungeon.Observe();
            return true;
        }

        public override void Detach()
        {
            Target.viewDistance = Dungeon.Level.viewDistance;
            Dungeon.Observe();
            base.Detach();
        }

        public override int Icon()
        {
            return BuffIndicator.LIGHT;
        }

        public override string ToString()
        {
            return "Illuminated";
        }
    }
}