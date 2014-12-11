using System.Collections.Generic;
using sharpdungeon.actors;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using Math = Java.Lang.Math;
using sharpdungeon.effects;
using pdsharp.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfDisintegration : Wand
    {
        public WandOfDisintegration()
        {
            name = "Wand of Disintegration";
            HitChars = false;
        }

        protected internal override void OnZap(int cell)
        {
            var terrainAffected = false;

            var localLevel = Level;

            var maxDistance = Distance();
            Ballistica.Distance = Math.Min(Ballistica.Distance, maxDistance);

            var chars = new List<Character>();

            for (var i=1; i < Ballistica.Distance; i++)
            {
                var c = Ballistica.Trace[i];

                Character ch;
                if ((ch = Actor.FindChar(c)) != null)
                    chars.Add(ch);

                var terr = Dungeon.Level.map[c];
                switch (terr)
                {
                    case Terrain.BARRICADE:
                    case Terrain.DOOR:
                        levels.Level.Set(c, Terrain.EMBERS);
                        GameScene.UpdateMap(c);
                        terrainAffected = true;
                        break;
                    case Terrain.HIGH_GRASS:
                        levels.Level.Set(c, Terrain.GRASS);
                        GameScene.UpdateMap(c);
                        terrainAffected = true;
                        break;
                }

                CellEmitter.Center(c).Burst(PurpleParticle.Burst, pdsharp.utils.Random.IntRange(1, 2));
            }

            if (terrainAffected)
                Dungeon.Observe();

            var lvl = localLevel + chars.Count;
            var dmgMin = lvl;
            var dmgMax = 8 + lvl * lvl / 3;
            foreach (var ch in chars)
            {
                ch.Damage(pdsharp.utils.Random.NormalIntRange(dmgMin, dmgMax), this);
                ch.Sprite.CenterEmitter().Burst(PurpleParticle.Burst, pdsharp.utils.Random.IntRange(1, 2));
                ch.Sprite.Flash();
            }
        }

        private int Distance()
        {
            return base.Level + 4;
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            cell = Ballistica.Trace[Math.Min(Ballistica.Distance, Distance()) - 1];
            CurUser.Sprite.Parent.Add(new DeathRay(CurUser.Sprite.Center(), DungeonTilemap.TileCenterToWorld(cell)));
            callback.Call();
        }

        public override string Desc()
        {
            return "This wand emits a beam of destructive energy, which pierces All creatures in its way. " + "The more targets it hits, the more damage it inflicts to each of them.";
        }
    }
}