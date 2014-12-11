using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;
using sharpdungeon.effects.particles;

namespace sharpdungeon.effects
{
    public class MagicMissile : Emitter
    {
        private const float Speed = 200f;

        private ICallback _callback;

        private float _sx;
        private float _sy;
        private float _time;
        
        public virtual void Reset(int from, int to, ICallback callback)
        {
            _callback = callback;

            Revive();

            var pf = DungeonTilemap.TileCenterToWorld(from);
            var pt = DungeonTilemap.TileCenterToWorld(to);

            x = pf.X;
            y = pf.Y;
            Width = 0;
            Height = 0;

            var d = PointF.Diff(pt, pf);
            var speed = new PointF(d).Normalize().Scale(Speed);
            _sx = speed.X;
            _sy = speed.Y;
            _time = d.Length / Speed;
        }

        public virtual void size(float size)
        {
            x -= size / 2;
            y -= size / 2;
            Width = Height = size;
        }

        public static void BlueLight(Group group, int from, int to, ICallback callback)
        {
            var missile = ((MagicMissile)group.Recycle(typeof(MagicMissile)));
            missile.Reset(from, to, callback);
            missile.Pour(MagicParticle.Factory, 0.01f);
        }

        public static void Fire(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(FlameParticle.Factory, 0.01f);
        }

        public static void Earth(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(2);
            missile.Pour(EarthParticle.Factory, 0.01f);
        }

        public static void PurpleLight(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(2);
            missile.Pour(PurpleParticle.Factory, 0.01f);
        }

        public static void WhiteLight(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(WhiteParticle.Factory, 0.01f);
        }

        public static void Wool(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(3);
            missile.Pour(WoolParticle.Factory, 0.01f);
        }

        public static void Poison(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(3);
            missile.Pour(PoisonParticle.Factory, 0.01f);
        }

        public static void Foliage(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(LeafParticle.Factory, 0.01f);
        }

        public static void Slowness(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.Pour(SlowParticle.Factory, 0.01f);
        }

        public static void Force(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(ForceParticle.Factory, 0.01f);
        }

        public static void ColdLight(Group group, int from, int to, ICallback callback)
        {
            var missile = @group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(ColdParticle.Factory, 0.01f);
        }

        public static void Shadow(Group group, int from, int to, ICallback callback)
        {
            var missile = group.Recycle<MagicMissile>();
            missile.Reset(from, to, callback);
            missile.size(4);
            missile.Pour(ShadowParticle.Missile, 0.01f);
        }

        public override void Update()
        {
            base.Update();
            
            if (!On) 
                return;

            var d = Game.Elapsed;
            x += _sx * d;
            y += _sy * d;
            
            if (!((_time -= d) <= 0)) 
                return;

            On = false;
            _callback.Call();
        }
    }
}