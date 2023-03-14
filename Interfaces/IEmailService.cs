namespace GuestSystemBack.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(string recipientAddress, string emailSubject, string emailBody);
    }
}
