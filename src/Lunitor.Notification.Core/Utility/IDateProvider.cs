using System;

namespace Lunitor.Notification.Core.Utility
{
    public interface IDateProvider
    {
        public DateTime Now { get; }
    }
}
