namespace AeroSharp.DataAccess.KeyValueAccess
{
    public class KeyValueStoreContext
    {
        public string[] Bins { get; }

        public KeyValueStoreContext(string[] bins)
        {
            Bins = bins;
        }
    }
}
