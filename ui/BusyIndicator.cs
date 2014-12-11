using pdsharp.noosa;

namespace sharpdungeon.ui
{
    public class BusyIndicator : Image
    {
        public BusyIndicator()
        {
            Copy(Icons.BUSY.Get());

            Origin.Set(Width / 2, Height / 2);
            AngularSpeed = 720;
        }

        public override void Update()
        {
            base.Update();
            Visible = Dungeon.Hero.IsAlive && !Dungeon.Hero.ready;
        }
    }
}