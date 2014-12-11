using pdsharp.noosa;
using sharpdungeon.windows;

namespace sharpdungeon.scenes
{
	public class IntroScene : PixelScene
	{
		private const string Text = "Many heroes of All kinds ventured into the Dungeon before you. Some of them have returned with treasures and magical " + 
            "artifacts, most have never been heard of since. But none have succeeded in retrieving the Amulet of Yendor, " +
            "which is told to be hidden in the depths of the Dungeon.\\Negative\\Negative" + 
            "You consider yourself ready for the challenge, but most importantly, you feel that fortune smiles on you. " + 
            "It's time to start your own adventure!";

		public override void Create()
		{
			base.Create();

		    Add(new IntroSceneWnd(Text));

			FadeIn();
		}
	}

    public class IntroSceneWnd : WndStory
    {
        public IntroSceneWnd(string text) : base(text)
        {
        }

        public override void Hide()
        {
            base.Hide(); 
            Game.SwitchScene(typeof(InterlevelScene));
        } 
    }

}