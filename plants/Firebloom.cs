using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.potions;
using sharpdungeon.scenes;
using sharpdungeon.sprites;

namespace sharpdungeon.plants
{
    public class Firebloom : Plant
    {
        public Firebloom()
        {
            Image = 0;
            PlantName = "Firebloom";
        }

        private const string TxtDesc = "When something touches a Firebloom, it bursts into flames.";

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            GameScene.Add(Blob.Seed(Pos, 2, typeof(Fire)));

            if (Dungeon.Visible[Pos])
                CellEmitter.Get(Pos).Burst(FlameParticle.Factory, 5);
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Firebloom";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_FIREBLOOM;

                PlantClass = typeof(Firebloom);
                AlchemyClass = typeof(PotionOfLiquidFlame);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }
    }
}