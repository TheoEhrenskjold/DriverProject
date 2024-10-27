using Microsoft.AspNetCore.Identity.UI.Services;
using System.Diagnostics;

namespace DriverProject.Data
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Log email details to the debug console for testing
            Debug.WriteLine($"Email sent to: {email}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {htmlMessage}");

            // Also output to the console for visibility in the development server logs
            Console.WriteLine("----- DUMMY EMAIL SENDER -----");
            Console.WriteLine($"Email sent to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {htmlMessage}");
            Console.WriteLine("----- END OF EMAIL -----");

            // Simulate async operation
            return Task.CompletedTask;
        }
    }
}
