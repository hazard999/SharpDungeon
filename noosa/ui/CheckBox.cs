namespace pdsharp.noosa.ui
{
    public class CheckBox : Button
    {
        private bool _checkState;

        public virtual bool CheckState
        {
            get { return _checkState; }
        }

        public virtual void SetCheckState(bool value)
        {
            if (_checkState == value) 
                return;

            _checkState = value;
            UpdateState();
        }

        protected internal virtual void UpdateState()
        {
        }

        protected override void OnClick()
        {
            SetCheckState(!_checkState);
            OnChange();
        }

        protected internal virtual void OnChange()
        {
        }
    }
}