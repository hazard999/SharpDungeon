using pdsharp.utils;

namespace pdsharp.noosa.tweeners
{
    public class CameraScrollTweener : Tweener
    {
        public PointF Start;
        public PointF End;

        public CameraScrollTweener(Camera camera, PointF pos, float time) : base(camera, time)
        {
            Start = camera.Scroll;
            End = pos;
        }

        protected override void UpdateValues(float progress)
        {
            Camera.Scroll = PointF.Inter(Start, End, progress);
        }
    }
}