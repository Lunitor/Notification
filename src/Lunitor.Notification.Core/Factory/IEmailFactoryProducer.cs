namespace Lunitor.Notification.Core.Factory
{
    internal interface IEmailFactoryProducer
    {
        EmailFactory GetEmailFactory(string templateType);
    }
}