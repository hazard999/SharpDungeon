using System;
using Android.App;
using pdsharp.input;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using pdsharp.utils;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class ScrollPane : Component
    {
        protected internal TouchController Controller;
        private readonly Component _content;

        protected internal float MinX;
        protected internal float MinY;
        protected internal float MaxX;
        protected internal float MaxY;

        public ScrollPane(Component content)
        {
            _content = content;
            AddToBack(content);

            _Width = content.Width;
            _Height = content.Height;

            content.Camera = new Camera(0, 0, 1, 1, PixelScene.defaultZoom);
            Camera.Add(content.Camera);
        }

        public override void Destroy()
        {
            base.Destroy();
            Camera.Remove(_content.Camera);
        }

        public virtual void ScrollTo(float x, float y)
        {
            _content.Camera.Scroll.Set(x, y);
        }

        protected override void CreateChildren()
        {
            Controller = new TouchController(this);
            Add(Controller);
        }

        protected override void Layout()
        {
            _content.SetPos(0, 0);
            Controller.X = X;
            Controller.Y = Y;
            Controller.Width = Width;
            Controller.Height = Height;

            var p = Camera.CameraToScreen(X, Y);
            var cs = _content.Camera;
            cs.X = p.X;
            cs.Y = p.Y;
            cs.Resize((int)Width, (int)Height);
        }

        public virtual Component Content()
        {
            return _content;
        }

        public Action<float, float> ClickAction;
        public virtual void OnClick(float x, float y)
        {
            if (ClickAction != null)
                ClickAction(x, y);
        }

        public class TouchController : TouchArea
        {
            private readonly ScrollPane _scrollPane;
            private readonly float _dragThreshold;

            public TouchController(ScrollPane scrollPane)
                : base(0, 0, 0, 0)
            {
                _scrollPane = scrollPane;
                _dragThreshold = PixelScene.defaultZoom * 8;
            }

            protected override void OnClick(Touch touch)
            {
                if (_dragging)
                    _dragging = false;
                else
                {
                    var p = _scrollPane.Content().Camera.ScreenToCamera((int)touch.Current.X, (int)touch.Current.Y);
                    _scrollPane.OnClick(p.X, p.Y);
                }
            }

            // true if dragging is in progress
            private bool _dragging;
            // last touch coords
            private readonly PointF _lastPos = new PointF();

            protected override void OnDrag(Touch t)
            {
                if (_dragging)
                {
                    var content = _scrollPane.Content();
                    var c = content.Camera;

                    c.Scroll.Offset(PointF.Diff(_lastPos, t.Current).InvScale(c.Zoom));

                    if (c.Scroll.X + Width > content.Width)
                        c.Scroll.X = content.Width - Width;

                    if (c.Scroll.X < 0)
                        c.Scroll.X = 0;

                    if (c.Scroll.Y + Height > content.Height)
                        c.Scroll.Y = content.Height - Height;

                    if (c.Scroll.Y < 0)
                        c.Scroll.Y = 0;

                    _lastPos.Set(t.Current);
                }
                else
                    if (PointF.Distance(t.Current, t.Start) > _dragThreshold)
                    {
                        _dragging = true;
                        _lastPos.Set(t.Current);

                    }
            }
        }
    }
}