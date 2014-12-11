using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfFirebolt : Wand
    {
        public WandOfFirebolt()
        {
            name = "Wand of Firebolt";
        }

        protected internal override void OnZap(int cell)
        {
            var localLevel = Level;

            for (var i=1; i < Ballistica.Distance - 1; i++)
            {
                var c = Ballistica.Trace[i];
                if (levels.Level.flamable[c])
                    GameScene.Add(Blob.Seed(c, 1, typeof (Fire)));
            }

            GameScene.Add(Blob.Seed(cell, 1, typeof(Fire)));

            var ch = Actor.FindChar(cell);
            if (ch == null) 
                return;

            ch.Damage(pdsharp.utils.Random.Int(1, 8 + localLevel * localLevel), this);
            Buff.Affect<Burning>(ch).Reignite(ch);

            ch.Sprite.Emitter().Burst(FlameParticle.Factory, 5);

            if (ch != CurUser || ch.IsAlive) 
                return;

            Dungeon.Fail(Utils.Format(ResultDescriptions.WAND, name, Dungeon.Depth));
            GLog.Negative("You killed yourself with your own Wand of Firebolt...");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Fire(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "This wand unleashes bursts of magical fire. It will ignite " + "flammable terrain, and will damage and burn a creature it hits.";
        }
    }
}