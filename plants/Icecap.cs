using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.items.potions;
using sharpdungeon.levels;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.plants
{
    public class Icecap : Plant
    {
        private const string TXT_DESC = "Upon touching an Icecap excretes a pollen, which freezes everything in its vicinity.";

        public Icecap()
        {
            Image = 1;
            PlantName = "Icecap";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            PathFinder.BuildDistanceMap(Pos, BArray.not(Level.losBlocking, null), 1);

            var fire = (Fire)Dungeon.Level.Blobs[typeof(Fire)];

            for (var i = 0; i < Level.Length; i++)
                if (PathFinder.Distance[i] < int.MaxValue)
                    Freezing.Affect(i, fire);
        }

        public override string Desc()
        {
            return TXT_DESC;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Icecap";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_ICECAP;

                PlantClass = typeof(Icecap);
                AlchemyClass = typeof(PotionOfFrost);
            }

            public override string Desc()
            {
                return TXT_DESC;
            }
        }
    }
}