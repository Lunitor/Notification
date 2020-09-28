namespace Lunitor.Notification.Core.Model
{
    public class EmailTemplate
    {
        public string Type { get; internal set; }
        public EmailTemplateContent Content { get; internal set; } = new EmailTemplateContent();

        public EmailTemplate(){}

        public EmailTemplate(string type, EmailTemplateContent templateContent)
        {
            Type = type;
            Content = templateContent;
        }

        public EmailTemplate(string type, string subject, string text)
        {
            Type = type;
            Content = new EmailTemplateContent(subject, text);
        }
    }
}