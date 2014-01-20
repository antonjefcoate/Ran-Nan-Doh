namespace RanNanDohUi.Infrastructure.WebUtils
{
    public interface ISessionState<T>
    {
        void Clear();
        void Delete(string key);
        T Get(string key);
        void Store(string key, T value);
    }
}