using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.items.potions;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.plants
{
    public class Dreamweed : Plant
    {
        private const string TxtDesc = "Upon touching a Dreamweed it secretes a glittering cloud of confusing gas.";

        public Dreamweed()
        {
            Image = 3;
            PlantName = "Dreamweed";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            if (ch != null)
                GameScene.Add(Blob.Seed(Pos, 300 + 20 * Dungeon.Depth, typeof(ConfusionGas)));
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Dreamweed";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_DREAMWEED;

                PlantClass = typeof(Dreamweed);
                AlchemyClass = typeof(PotionOfInvisibility);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }
    }
}