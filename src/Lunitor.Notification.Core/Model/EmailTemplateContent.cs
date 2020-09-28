namespace Lunitor.Notification.Core.Model
{
    public class EmailTemplateContent
    {
        public string Subject { get; internal set; }
        public string Text { get; internal set; }

        public EmailTemplateContent(){}

        public EmailTemplateContent(string subject, string text)
        {
            Subject = subject;
            Text = text;
        }
    }
}