namespace pdsharp.utils
{
	public interface Bundlable
	{
		void RestoreFromBundle(Bundle bundle);
		void StoreInBundle(Bundle bundle);
	}
}