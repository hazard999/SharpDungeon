using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.potions;
using sharpdungeon.sprites;

namespace sharpdungeon.plants
{
    public class Sorrowmoss : Plant
    {
        private const string TxtDesc = "A Sorrowmoss is a flower (not a moss) with razor-sharp petals, coated with a deadly venom.";

        public Sorrowmoss()
        {
            Image = 2;
            PlantName = "Sorrowmoss";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            if (ch != null)
                Buff.Affect<Poison>(ch).Set(Poison.DurationFactor(ch) * (4 + Dungeon.Depth / 2));

            if (Dungeon.Visible[Pos])
                CellEmitter.Center(Pos).Burst(PoisonParticle.Splash, 3);
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Sorrowmoss";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_SORROWMOSS;

                PlantClass = typeof(Sorrowmoss);
                AlchemyClass = typeof(PotionOfToxicGas);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }
    }
}