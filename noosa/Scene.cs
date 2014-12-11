using pdsharp.input;
using pdsharp.utils;

namespace pdsharp.noosa
{
    public class Scene : Group, IListener<Key>
    {
        public virtual void Create()
        {
            Keys.Event.Add(this);
        }

        public override void Destroy()
        {
            Keys.Event.Remove(this);
            base.Destroy();
        }

        public virtual void Pause()
        {

        }

        public virtual void Resume()
        {

        }

        public override Camera Camera
        {
            get { return Camera.Main; }
        }

        protected virtual void OnBackPressed()
        {
            Game.Instance.Finish();
        }

        protected virtual void OnMenuPressed()
        {

        }

        public void OnSignal(Key key)
        {
            if (Game.Instance == null || !key.Pressed) 
                return;

            switch (key.Code)
            {
                case Keys.Back:
                    OnBackPressed();
                    break;
                case Keys.Menu:
                    OnMenuPressed();
                    break;
            }
        }
    }
}