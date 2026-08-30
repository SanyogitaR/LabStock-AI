using Microsoft.AspNetCore.Identity.UI.Services;

namespace InventoryManagement.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            Console.WriteLine($"Email to: {email}");
            Console.WriteLine($"Subject: {subject}");

            return Task.CompletedTask;
        }
    }
}