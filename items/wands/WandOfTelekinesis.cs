using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.items.scrolls;
using sharpdungeon.levels;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfTelekinesis : Wand
    {
        private const string TxtYouNowHave = "You have magically transported {0} into your backpack";

        public WandOfTelekinesis()
        {
            name = "Wand of Telekinesis";
            HitChars = false;
        }

        protected internal override void OnZap(int cell)
        {
            var mapUpdated = false;

            var maxDistance = Level + 4;
            Ballistica.Distance = System.Math.Min(Ballistica.Distance, maxDistance);

            Heap heap = null;

            for (var i = 1; i < Ballistica.Distance; i++)
            {
                var c = Ballistica.Trace[i];

                var before = Dungeon.Level.map[c];

                Character ch;
                if ((ch = Actor.FindChar(c)) != null)
                {

                    if (i == Ballistica.Distance - 1)
                    {
                        ch.Damage(maxDistance - 1 - i, this);
                    }
                    else
                    {
                        var next = Ballistica.Trace[i + 1];
                        if ((levels.Level.passable[next] || levels.Level.avoid[next]) && Actor.FindChar(next) == null)
                        {
                            Actor.AddDelayed(new Pushing(ch, ch.pos, next), -1);

                            ch.pos = next;
                            Actor.FreeCell(next);

                            // FIXME
                            var mob = ch as Mob;
                            if (mob != null)
                                Dungeon.Level.MobPress(mob);
                            else
                                Dungeon.Level.Press(ch.pos, ch);
                        }
                        else
                            ch.Damage(maxDistance - 1 - i, this);
                    }
                }

                if (heap == null && (heap = Dungeon.Level.heaps[c]) != null)
                {
                    switch (heap.HeapType)
                    {
                        case Heap.Type.Heap:
                            Transport(heap);
                            break;
                        case Heap.Type.Chest:
                            Open(heap);
                            break;
                    }
                }

                Dungeon.Level.Press(c, null);
                if (before == Terrain.OPEN_DOOR && Actor.FindChar(c) == null)
                {
                    levels.Level.Set(c, Terrain.DOOR);
                    GameScene.UpdateMap(c);
                }
                else
                    if (levels.Level.water[c])
                        GameScene.Ripple(c);

                if (!mapUpdated && Dungeon.Level.map[c] != before)
                    mapUpdated = true;
            }

            if (mapUpdated)
                Dungeon.Observe();
        }

        private void Transport(Heap heap)
        {
            var item = heap.PickUp();
            if (item.DoPickUp(CurUser))
            {
                if (item is Dewdrop) 
                    return;

                if ((item is ScrollOfUpgrade && ((ScrollOfUpgrade) item).IsKnown) || (item is PotionOfStrength && ((PotionOfStrength) item).IsKnown))
                    GLog.Positive(TxtYouNowHave, item.Name);
                else
                    GLog.Information(TxtYouNowHave, item.Name);
            }
            else
                Dungeon.Level.Drop(item, CurUser.pos).Sprite.Drop();
        }

        private void Open(Heap heap)
        {
            heap.HeapType = Heap.Type.Heap;
            heap.Sprite.Link();
            heap.Sprite.Drop();
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Force(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "Waves of magic force from this wand will affect All cells on their way triggering traps, trampling high vegetation, " + "opening closed doors and closing open ones. They also push back monsters.";
        }
    }
}