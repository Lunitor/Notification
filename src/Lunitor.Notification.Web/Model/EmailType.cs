using System.Collections.Generic;

namespace Lunitor.Notification.Web.Model
{
    public class EmailType
    {
        public string Name { get; set; }
        public IDictionary<string, string> Placeholders { get; set; }
    }
}
