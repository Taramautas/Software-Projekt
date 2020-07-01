using System;
using System.Net;
using System.Net.Mail;


namespace Uebungsprojekt.Service
{
    
    
    public class NotificationService
    {
        private static string sender_mail = "emailcheckaspnet@gmail.com";
        private static string reciever_mail = "emailcheckaspnet@gmail.com";
        private static string mail_password = "";    
        
        
        /// <summary>
        /// Creates a MailMessage and fills it with the given information. Finally sends it via a smtp Connection
        /// </summary>
        /// TODO: fill Method with variable sender_mail, reciever_mail, pwd
        public void SendEmail()
        {
            using (MailMessage mm = new MailMessage(sender_mail, reciever_mail))
            {
                mm.Subject = "Sent from ASP.NET Server";
                mm.Body = "Hallo Radi,\nWollte dir nur mitteilen das E-mail notifications nun möglich sind." +
                          "\nViele Grüße\n\nDomi";
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(sender_mail, mail_password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        
    }
}