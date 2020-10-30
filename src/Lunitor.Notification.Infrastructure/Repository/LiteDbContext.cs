using Ardalis.GuardClauses;
using LiteDB;
using Lunitor.Notification.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Lunitor.Notification.Infrastructure.Repository
{
    internal class LiteDbContext : ILiteDbContext
    {
        public ILiteDatabase Database { get; }

        public LiteDbContext(IOptions<LiteDbConfiguration> dbConfiguration)
        {
            Guard.Against.Null(dbConfiguration, nameof(dbConfiguration));

            Database = new LiteDatabase(dbConfiguration.Value.DatabasePath);
        }
    }
}
