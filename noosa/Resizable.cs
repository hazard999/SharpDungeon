namespace pdsharp.noosa
{
	public interface Resizable
	{
		void Size(float Width, float Height);
		float Width { get; }
		float Height { get; }
	}
}