using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InstituteOfFineArts.Models
{
    public class EmailCofirm
    {
        public void SendMail(string receiveMail, string receiveName, string mailContent)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            MailAddress from = new MailAddress("Vienlvd00597@fpt.edu.vn", "My Admin");
            MailAddress to = new MailAddress(receiveMail, receiveName);
            MailMessage message = new MailMessage(from, to);
            message.Body = mailContent;
            message.IsBodyHtml = true;
            message.Subject = "Gmail contest announcement";
            NetworkCredential myCreds = new NetworkCredential("Vienlvd00597@fpt.edu.vn", "kkaqslvrjforxixo", "");
            client.Credentials = myCreds;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception is:" + ex.ToString());
            }
            Console.WriteLine("Goodbye.");
        }
    }
}
