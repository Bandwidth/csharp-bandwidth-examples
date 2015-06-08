using System;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TranscriptionApp.Lib
{
    public sealed class EmailSender: IDisposable
    {
        public static EmailSender Create()
        {
            return new EmailSender();
        }

        public Task SendMessage(string email, string subject, string text)
        {
            return Task.Run(() =>
            {
                var message = new MailMessage(ConfigurationManager.AppSettings["from"], email, subject, text);
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(message);
                }                
            });
        }
        
        public void Dispose()
        {
        }
    }
}