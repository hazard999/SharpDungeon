using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.actors.mobs;

namespace sharpdungeon.sprites
{
    public class MobSprite : CharSprite
    {
        private const float FadeTime = 3f;
        private const float FallTime = 1f;

        public override void Update()
        {
            Sleeping = Ch != null && ((Mob)Ch).State == ((Mob)Ch).Sleepeing;
            base.Update();
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);
            
            if (anim != DieAnimation) 
                return;
            var tweener = new AlphaTweener(this, 0, FadeTime);
            tweener.CompleteAction = action => tweener.KillAndErase();

            Parent.Add(tweener);
        }

        public virtual void Fall()
        {
            Origin.Set(Width / 2, Height - DungeonTilemap.Size / 2);
            AngularSpeed = Random.Int(2) == 0 ? -720 : 720;
            var scaleTweener = new ScaleTweener(this, new PointF(0, 0), FallTime);
            scaleTweener.CompleteAction = action => scaleTweener.KillAndErase();
            scaleTweener.UpdateValuesAction = progress => Am = 1 - progress;
            Parent.Add(scaleTweener);
        }

        public virtual void Attack(int cell)
        {
            throw new System.NotImplementedException();
        }
    }

}