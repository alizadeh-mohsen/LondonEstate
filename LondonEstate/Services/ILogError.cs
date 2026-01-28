namespace LondonEstate.Services
{
    public interface ILogError
    {
        Task LogErrorToDb(Exception ex,string area);
    }
}
