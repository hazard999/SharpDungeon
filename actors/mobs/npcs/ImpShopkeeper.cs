using System.Linq;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.actors.mobs.npcs
{
    public class ImpShopkeeper : Shopkeeper
    {
        private const string TxtGreetings = "Hello, friend!";

        public ImpShopkeeper()
        {
            Name = "ambitious imp";
            SpriteClass = typeof(ImpSprite);
        }

        private bool _seenBefore;

        protected override bool Act()
        {
            if (_seenBefore || !Dungeon.Visible[pos])
                return base.Act();

            Yell(Utils.Format(TxtGreetings));
            _seenBefore = true;

            return base.Act();
        }

        protected internal override void Flee()
        {
            foreach (var heap in Dungeon.Level.heaps.Values.Where(heap => heap.HeapType == Heap.Type.ForSale))
            {
                CellEmitter.Get(heap.Pos).Burst(ElmoParticle.Factory, 4);
                heap.Destroy();
            }

            Destroy();

            Sprite.Emitter().Burst(Speck.Factory(Speck.WOOL), 15);
            Sprite.KillAndErase();
        }

        public override string Description()
        {
            return "Imps are lesser demons. They are notable for neither their strength nor their magic talent. " +
                "But they are quite smart and sociable, and many of imps prefer to live and do business among non-demons.";
        }
    }
}