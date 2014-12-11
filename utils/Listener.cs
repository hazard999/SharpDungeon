namespace pdsharp.utils
{
    public interface IListener<T>
    {
        void OnSignal(T t);
    }
}