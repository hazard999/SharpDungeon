using pdsharp.utils;
using sharpdungeon.levels;

namespace sharpdungeon.actors.mobs.npcs
{
    public abstract class NPC : Mob
    {
        protected NPC()
        {
            HP = HT = 1;
            Exp = 0;

            Hostile = false;
            State = PASSIVE;
        }

        protected internal virtual void ThrowItem()
        {
            var heap = Dungeon.Level.heaps[pos];
            if (heap == null) 
                return;

            int n;
            do
            {
                n = pos + Level.NEIGHBOURS8[Random.Int(8)];
            } while (!Level.passable[n] && !Level.avoid[n]);
            Dungeon.Level.Drop(heap.PickUp(), n).Sprite.Drop(pos);
        }

        public override void Beckon(int cell)
        {
        }

        public abstract void Interact();
        public new abstract bool Reset();
    }

}