
using LondonEstate.Data;
using LondonEstate.Models;
using Serilog;

namespace LondonEstate.Services
{
    public class LogError : ILogError
    {
        private readonly ApplicationDbContext _dbContext;

        public LogError(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task LogErrorToDb(Exception ex, string area)
        {
            try
            {
                var errorLog = new ErrorLog
                {
                    Message = ex.Message,
                    ErrorDetail = ex.ToString(),
                    Area = area
                };

                _dbContext.ErrorLogs.Add(errorLog);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                Log.Error("<<<<<<< Failed to log error to database: {ErrorMessage}", ex.Message);
            }
        }

    }
}
