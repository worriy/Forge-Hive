
using System;
using System.Net;
using System.Threading.Tasks;
using Hive.Backend.Infrastructure.Services;
using Hive.Backend.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Hive.Backend.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            return ExecuteWithMailKit(subject, message, email);
        }

        /*public Task ExecuteWithSmtp(string subject, string message, string email)
        {
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    //client.Host = "smtp-mail.outlook.com";
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("notificationshive@gmail.com", "jsP7j{kWT8bn&TbF"),
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("notificationshive@gmail.com", "Notification - Hive")
                };
                mailMessage.To.Add(email);
                mailMessage.Body = message;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;

                client.Send(mailMessage);
                return Task.FromResult(0);
            }
            catch (Exception xcp)
            {
                return Task.FromException(xcp);
            }
        }*/

        public Task ExecuteWithMailKit(string subject, string message, string email)
        {
            try
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress("Notification - Hive", "notificationshive@gmail.com"));
                mailMessage.To.Add(new MailboxAddress(email, email));
                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart()
                {
                    Text = message
                };

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate("notificationshive@gmail.com", "jsP7j{kWT8bn&TbF");
                    smtpClient.Send(mailMessage);
                    smtpClient.Disconnect(true);
                }
                return Task.FromResult(0);
            }
            catch (Exception xcp)
            {
                return Task.FromException(xcp);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
