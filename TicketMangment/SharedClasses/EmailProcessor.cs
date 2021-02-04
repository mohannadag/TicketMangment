using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketMangment.SharedClasses
{
    public static class EmailProcessor
    {
        public static void SendEmail(string name, string email, string subject, string body)
        {
            // Prepare an email message to be sent

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("Ticket Mangment System", "ag.mohannad1@gmail.com");
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(name, email);
            message.To.Add(to);

            message.Subject = subject;

            // Add email body and file attachments

            BodyBuilder bodyBuilder = new BodyBuilder();
            //bodyBuilder.HtmlBody = "<h1>Hello from the ticket mangment system</h1>";
            bodyBuilder.TextBody = body;

            // if we need to  add attachments with the email we uncomment the next line and specify the file we want
            //bodyBuilder.Attachments.Add(hostingEnvironment.WebRootPath + "\\file.png");

            message.Body = bodyBuilder.ToMessageBody();

            // Connect and authenticate with the SMTP server

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate("ag.mohannad1@gmail.com", "!99#Mhnd");

            // Send email message

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }
    }
}
