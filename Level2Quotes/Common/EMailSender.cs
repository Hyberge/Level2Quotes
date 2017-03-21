using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace Level2Quotes
{
    class EMailSender
    {
        static String mFromAddress = "haizhishen-11-7@163.com";
        static String mToAddress = "mabaichuan@kingsoft.com";
        static String mClientAddress = "smtp.163.com";
        static String mUserName = "haizhishen-11-7";
        static String mPassWD = "xiaoxifu";

        public static bool SenderMessage(String Subject, String Message)
        {
            bool ret = true;

            SmtpClient Client = new SmtpClient(mClientAddress);

            MailAddress From = new MailAddress(mFromAddress);
            MailAddress To = new MailAddress(mToAddress);

            MailMessage EMail = new MailMessage(From, To);

            EMail.Subject = Subject;
            EMail.SubjectEncoding = Encoding.UTF8;
            EMail.Body = Message;
            EMail.BodyEncoding = Encoding.UTF8;
            EMail.IsBodyHtml = false;
            EMail.Priority = MailPriority.High;

            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
            Client.UseDefaultCredentials = true;
            Client.Credentials = new NetworkCredential(mUserName, mPassWD);

            try
            {
            	Client.Send(EMail);
            }
            catch (System.Exception ex)
            {
                ret = false;
                Console.WriteLine(ex.Message);
            }

            return ret;
        }
    }
}
