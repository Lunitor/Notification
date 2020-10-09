namespace Lunitor.Notification.Core.Model
{
    public class SendingResult
    {
        public string EmailAddress { get; set; }
        public bool IsSuccess { get; set; }

        public SendingResult(string emailAddress, bool isSuccess)
        {
            EmailAddress = emailAddress;
            IsSuccess = isSuccess;
        }
    }
}
