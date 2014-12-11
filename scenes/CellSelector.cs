using System;
using pdsharp.input;
using pdsharp.noosa;
using pdsharp.utils;

namespace sharpdungeon.scenes
{
    public class CellSelector : TouchArea
    {
        public Listener listener = null;

        public bool enabled;

        private readonly float _dragThreshold;

        public CellSelector(DungeonTilemap map)
            : base(map)
        {
            Camera = map.Camera;

            _dragThreshold = PixelScene.defaultZoom * DungeonTilemap.Size / 2;
        }

        protected override void OnClick(Touch touch)
        {
            if (_dragging)
                _dragging = false;
            else
                Select(((DungeonTilemap)Target).ScreenToTile((int)touch.Current.X, (int)touch.Current.Y));
        }

        public virtual void Select(int cell)
        {
            if (enabled && listener != null && cell != -1)
            {
                listener.OnSelect(cell);
                GameScene.Ready();
            }
            else
                GameScene.Cancel();
        }

        private bool _pinching;
        private Touch _another;
        private float _startZoom;
        private float _startSpan;

        protected override void OnTouchDown(Touch t)
        {
            if (t == Touch || _another != null) 
                return;

            if (!Touch.Down)
            {
                Touch = t;
                OnTouchDown(t);
                return;
            }

            _pinching = true;

            _another = t;
            _startSpan = PointF.Distance(Touch.Current, _another.Current);
            _startZoom = Camera.Zoom;

            _dragging = false;
        }

        protected override void OnTouchUp(Touch t)
        {
            if (!_pinching || (t != Touch && t != _another)) 
                return;

            _pinching = false;

            var zoom = (float)Math.Round(Camera.Zoom);
            Camera.ZoomTo(zoom);
            PixelDungeon.Zoom((int)(zoom - PixelScene.defaultZoom));

            _dragging = true;
            if (t == Touch)
                Touch = _another;
            _another = null;
            _lastPos.Set(Touch.Current);
        }

        private bool _dragging;
        private readonly PointF _lastPos = new PointF();

        protected override void OnDrag(Touch t)
        {
            Camera.Target = null;

            if (_pinching)
            {
                var curSpan = PointF.Distance(Touch.Current, _another.Current);
                Camera.ZoomTo(GameMath.Gate(PixelScene.minZoom, _startZoom * curSpan / _startSpan, PixelScene.maxZoom));
            }
            else
            {
                if (!_dragging && PointF.Distance(t.Current, t.Start) > _dragThreshold)
                {
                    _dragging = true;
                    _lastPos.Set(t.Current);
                }
                else if (_dragging)
                {
                    Camera.Scroll.Offset(PointF.Diff(_lastPos, t.Current).InvScale(Camera.Zoom));
                    _lastPos.Set(t.Current);
                }
            }
        }

        public virtual void Cancel()
        {
            if (listener != null)
                listener.OnSelect(null);

            GameScene.Ready();
        }

        public interface Listener
        {
            void OnSelect(int? target);
            string Prompt();
        }
    }
}