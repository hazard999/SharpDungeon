using sharpdungeon.actors;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.items.scrolls;
using sharpdungeon.sprites;

namespace sharpdungeon.plants
{
    public class Fadeleaf : Plant
    {
        private const string TxtDesc = "Touching a Fadeleaf will teleport any creature " + "to a Random place on the current Level.";

        public Fadeleaf()
        {
            Image = 6;
            PlantName = "Fadeleaf";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            var hero = ch as Hero;
            if (hero != null)
            {
                ScrollOfTeleportation.TeleportHero(hero);
                hero.curAction = null;
            }
            else
                if (ch is Mob)
                {
                    // Why do I try to choose a new position 10 times?
                    // I don't remember...
                    var count = 10;
                    int newPos;
                    do
                    {
                        newPos = Dungeon.Level.RandomRespawnCell();

                        if (count-- <= 0)
                            break;

                    } while (newPos == -1);

                    if (newPos != -1)
                    {
                        ch.pos = newPos;
                        ch.Sprite.Place(ch.pos);
                        ch.Sprite.Visible = Dungeon.Visible[Pos];
                    }
                }

            if (Dungeon.Visible[Pos])
                CellEmitter.Get(Pos).Start(Speck.Factory(Speck.LIGHT), 0.2f, 3);
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Fadeleaf";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_FADELEAF;

                PlantClass = typeof(Fadeleaf);
                AlchemyClass = typeof(PotionOfMindVision);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }
    }
}