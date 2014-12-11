using System;
using pdsharp.utils;

namespace pdsharp.noosa.tweeners
{
    public class ScaleTweener : Tweener
    {
        public Visual Visual;

        public PointF Start;
        public PointF End;

        public ScaleTweener(Visual visual, PointF scale, float time)
            : base(visual, time)
        {

            Visual = visual;
            Start = visual.Scale;
            End = scale;
        }

        public Action<float> UpdateValuesAction;

        protected override void UpdateValues(float progress)
        {
            Visual.Scale = PointF.Inter(Start, End, progress);

            if (UpdateValuesAction != null)
                UpdateValuesAction(progress);
        }
    }
}