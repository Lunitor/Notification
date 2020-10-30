using Lunitor.Notification.Core.Utility;
using System;

namespace Lunitor.Notification.Infrastructure.Utility
{
    class SystemDateProvider : IDateProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
