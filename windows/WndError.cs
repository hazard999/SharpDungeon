using sharpdungeon.ui;

namespace sharpdungeon.windows
{
	public class WndError : WndTitledMessage
	{
		private const string TxtTitle = "ERROR";

		public WndError(string message) : base(Icons.WARNING.Get(), TxtTitle, message)
		{
		}
	}
}