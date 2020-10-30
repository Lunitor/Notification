using LiteDB;

namespace Lunitor.Notification.Infrastructure.Repository
{
    public interface ILiteDbContext
    {
        public ILiteDatabase Database { get; }
    }
}
