using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Services
{
    public class EmailServiceMock : IEmailService
    {
        public void SendEmail(string recipientAddress, string emailSubject, string emailBody)
        {
            //We do a little bit of mocking
        }
    }
}
