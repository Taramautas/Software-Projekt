using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace TMP_EMAIL_CHECK
{
    class Program
    {
        
        
        private static string from = "emailcheckaspnet@gmail.com";
        private static string to = "radi2000@hotmail.de";
        private static string mail_password;
        
        
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Credentials for:"+from);
            mail_password = Console.ReadLine();
            SendEmail();
        }
        
        /// <summary>
        /// Creates a MailMessage and fills it with the given information. Finally sends it via a smtp Connection
        /// </summary>
        protected static void SendEmail()
        {
            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = "Sent from ASP.NET Server";
                mm.Body = "Hallo Radi,\nWollte dir nur mitteilen das E-mail notifications nun möglich sind." +
                          "\nViele Grüße\n\nDomi";
           
                mm.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential(from, mail_password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

    }
}