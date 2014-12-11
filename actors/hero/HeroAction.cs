using sharpdungeon.actors.mobs.npcs;

namespace sharpdungeon.actors.hero
{
    public class HeroAction
    {
        public int Dst;

        public class Move : HeroAction
        {
            public Move(int dst)
            {
                Dst = dst;
            }
        }

        public class PickUp : HeroAction
        {
            public PickUp(int dst)
            {
                Dst = dst;
            }
        }

        public class OpenChest : HeroAction
        {
            public OpenChest(int dst)
            {
                Dst = dst;
            }
        }

        public class Buy : HeroAction
        {
            public Buy(int dst)
            {
                Dst = dst;
            }
        }

        public class Interact : HeroAction
        {
            public NPC Npc;
            
            public Interact(NPC npc)
            {
                Npc = npc;
            }
        }

        public class Unlock : HeroAction
        {
            public Unlock(int door)
            {
                this.Dst = door;
            }
        }

        public class Descend : HeroAction
        {
            public Descend(int stairs)
            {
                this.Dst = stairs;
            }
        }

        public class Ascend : HeroAction
        {
            public Ascend(int stairs)
            {
                Dst = stairs;
            }
        }

        public class Cook : HeroAction
        {
            public Cook(int pot)
            {
                Dst = pot;
            }
        }

        public class Attack : HeroAction
        {
            public Character Target;
            public Attack(Character target)
            {
                Target = target;
            }
        }
    }
}