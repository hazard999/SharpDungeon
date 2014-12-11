using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon.actors.blobs
{
    public class WaterOfAwareness : WellWater
    {
        private const string TXT_PROCCED = "As you take a sip, you feel the knowledge pours into your mind. " + "Now you know everything about your equipped items. Also you sense " + "All items on the Level and know All its secrets.";

        protected internal override bool AffectHero(Hero hero)
        {
            Sample.Instance.Play(Assets.SND_DRINK);
            Emitter.Parent.Add(new Identification(DungeonTilemap.TileCenterToWorld(Pos)));

            hero.Belongings.Observe();

            for (var i = 0; i < Level.Length; i++)
            {
                var terr = Dungeon.Level.map[i];

                if ((Terrain.Flags[terr] & Terrain.SECRET) == 0)
                    continue;

                Level.Set(i, Terrain.discover(terr));
                GameScene.UpdateMap(i);

                if (Dungeon.Visible[i])
                    GameScene.DiscoverTile(i, terr);
            }

            Buff.Affect<Awareness>(hero, Awareness.Duration);
            Dungeon.Observe();

            Dungeon.Hero.Interrupt();

            GLog.Positive(TXT_PROCCED);

            Journal.Remove(Journal.Feature.WELL_OF_AWARENESS);

            return true;
        }

        protected internal override Item AffectItem(Item item)
        {
            if (item.Identified)
                return null;
            
            item.Identify();
            Badge.ValidateItemLevelAquired(item);

            Emitter.Parent.Add(new Identification(DungeonTilemap.TileCenterToWorld(Pos)));

            Journal.Remove(Journal.Feature.WELL_OF_AWARENESS);

            return item;
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);
            emitter.Pour(Speck.Factory(Speck.QUESTION), 0.3f);
        }

        public override string TileDesc()
        {
            return "Power of knowledge radiates from the water of this well. " + "Take a sip from it to reveal All secrets of equipped items.";
        }
    }
}