using System;
using System.Net;
using System.Net.Mail;


namespace Uebungsprojekt.Service
{
    
    
    public class NotificationService
    {
        private static string sender_mail = "emailcheckaspnet@gmail.com";
        //     private static string reciever_mail = "emailcheckaspnet@gmail.com";
        private static string mail_password = "HelloWorld!12";


        /// <summary>
        /// Creates a MailMessage and fills it with the given information. Finally sends it via a smtp Connection
        /// </summary>
        /// TODO: fill Method with variable sender_mail, reciever_mail, pwd
        public void SendEmail(String reciever_mail, String Name, String city)
        {
            using (MailMessage mm = new MailMessage(sender_mail, reciever_mail))
            {
                mm.Subject = "Sent from ASP.NET Server";
                mm.Body = "Hallo "+Name+",\n ihr Ladeslot in "+city+" an der Zone - beginnt in 15 minuten" +
                          "\nViele Grüße\n\nihr Team 11";
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