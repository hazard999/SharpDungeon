
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfRegrowth : Wand
    {
        public WandOfRegrowth()
        {
            name = "Wand of Regrowth";
        }

        protected internal override void OnZap(int cell)
        {
            for (var i = 1; i < Ballistica.Distance - 1; i++)
            {
                var p = Ballistica.Trace[i];
                var c1 = Dungeon.Level.map[p];

                if (c1 == Terrain.EMPTY || c1 == Terrain.EMBERS || c1 == Terrain.EMPTY_DECO)
                    levels.Level.Set(p, Terrain.GRASS);
            }

            var c = Dungeon.Level.map[cell];
            if (c == Terrain.EMPTY || c == Terrain.EMBERS || c == Terrain.EMPTY_DECO || c == Terrain.GRASS || c == Terrain.HIGH_GRASS)
                GameScene.Add(Blob.Seed(cell, (Level + 2) * 20, typeof(Regrowth)));
            else
                GLog.Information("nothing happened");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Foliage(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "\"When life ceases new life always begins to grow... The eternal cycle always remains!\"";
        }
    }
}