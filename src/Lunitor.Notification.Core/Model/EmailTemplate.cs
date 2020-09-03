namespace Lunitor.Notification.Core.Model
{
    internal class EmailTemplate
    {
        public string Type { get; internal set; }
        public EmailTemplateContent TemplateContent { get; }
        public EmailTemplateContent Content { get; internal set; } = new EmailTemplateContent();

        public EmailTemplate(){}

        public EmailTemplate(string type, EmailTemplateContent templateContent)
        {
            Type = type;
            TemplateContent = templateContent;
        }

        public EmailTemplate(string type, string subject, string text)
        {
            Type = type;
            Content = new EmailTemplateContent(subject, text);
        }
    }
}