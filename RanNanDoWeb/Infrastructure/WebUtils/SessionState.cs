using System.Web;

namespace RanNanDohUi.Infrastructure.WebUtils
{
    public class SessionState<T> : ISessionState<T>
    {
        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string key)
        {
            throw new System.NotImplementedException();
        }

        public T Get(string key)
        {
            return (T)HttpContext.Current.Session[key];
        }

        public void Store(string key, T value)
        {
            HttpContext.Current.Session[key] = value;
        }
    }
}