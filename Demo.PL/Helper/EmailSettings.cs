using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com" , 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("kirollos.samir766@gmail.com", "hfwtesdqmxirhwsf");
            Client.Send("kirollos.samir766@gmail.com", email.To, email.Subject, email.Body);


        }
    }
}
