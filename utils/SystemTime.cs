namespace pdsharp.utils
{
    public class SystemTime
    {
        public static long Now;

        public static void Tick()
        {
            Now = System.DateTime.Now.Ticks / 16666;
        }
    }
}