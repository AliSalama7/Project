namespace Project.Services
{
    public interface ISensorDataReader
    {
        Task<string> ReadLineAsync();
        void Dispose();
    }
}
