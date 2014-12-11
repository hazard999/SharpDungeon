using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.rings;
using sharpdungeon.scenes;

namespace sharpdungeon.levels.features
{
    public class HighGrass
    {
        public static void Trample(Level level, int pos, Character ch)
        {
            Level.Set(pos, Terrain.GRASS);
            GameScene.UpdateMap(pos);

            if (!Dungeon.IsChallenged(Challenges.NO_HERBALISM))
            {
                var herbalismLevel = 0;
                if (ch != null)
                {
                    var herbalism = ch.Buff<RingOfHerbalism.Herbalism>();
                    if (herbalism != null)
                        herbalismLevel = herbalism.Level;
                }

                // Seed
                if (herbalismLevel >= 0 && pdsharp.utils.Random.Int(18) <= pdsharp.utils.Random.Int(herbalismLevel + 1))
                    level.Drop(Generator.Random(Generator.Category.SEED), pos).Sprite.Drop();

                // Dew
                if (herbalismLevel >= 0 && pdsharp.utils.Random.Int(6) <= pdsharp.utils.Random.Int(herbalismLevel + 1))
                    level.Drop(new Dewdrop(), pos).Sprite.Drop();
            }

            var leaves = 4;

            // Barkskin
            var hero = ch as Hero;
            if (hero != null && hero.subClass == HeroSubClass.WARDEN)
            {
                Buff.Affect<Barkskin>(hero).Level(hero.HT / 3);
                leaves = 8;
            }

            CellEmitter.Get(pos).Burst(LeafParticle.LevelSpecific, leaves);
            Dungeon.Observe();
        }
    }

}