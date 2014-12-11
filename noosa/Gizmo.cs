namespace pdsharp.noosa
{

    public class Gizmo
    {
        public bool Exists;
        public bool Alive;
        public bool Active;
        public bool Visible;

        public Group Parent;

        protected Camera _Camera;

        public Gizmo()
        {
            Exists = true;
            Alive = true;
            Active = true;
            Visible = true;
        }

        public virtual void Destroy()
        {
            Parent = null;
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void Kill()
        {
            Alive = false;
            Exists = false;
        }

        // Not exactly opposite to "kill" method
        public virtual void Revive()
        {
            Alive = true;
            Exists = true;
        }

        public virtual Camera Camera
        {
            get
            {
                if (_Camera != null)
                    return _Camera;

                if (Parent != null)
                    return Parent.Camera;

                return null;
            }
            set { _Camera = value; }
        }

        public virtual bool IsVisible
        {
            get
            {
                if (Parent == null)
                    return Visible;

                return Visible && Parent.Visible;
            }
        }

        public virtual bool IsActive
        {
            get
            {
                if (Parent == null)
                    return Active;

                return Active && Parent.Active;
            }
            set { Active = value; }
        }

        public virtual void KillAndErase()
        {
            Kill();

            if (Parent != null)
                Parent.Erase(this);
        }

        public virtual void Remove()
        {
            if (Parent != null)
                Parent.Remove(this);
        }
    }
}