using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.repository;
using ElmahCore;
using Microsoft.AspNetCore.Http;

namespace rkbc.core.service
{
    public class EmailWrapper 
    {
        public EmailWrapper(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            
        }

        public EmailSettings _emailSettings { get; }

        public static void SendEmail(SmtpClient client, MailMessage msg)
        {
            //Test settings - unfilter this when we go live, this just allows transfer to AMEC addresses.
            //var rlst = msg.To.ToList();
            //msg.To.Clear();
            //foreach (var e in rlst)
            //{
            //    if (e.Address.ToLower().EndsWith("amec.com"))
            //    {
            //        msg.To.Add(new MailAddress(e.Address));
            //    }
            //}
            //if (msg.To.Count() == 0) return;

            //Debug server settings
            //if (ConfigurationManager.AppSettings["EmailDebugMode"] == "true")
            //{
            //    string recip = "";
            //    foreach (var e in msg.To) { recip = recip + e.Address + ", "; }
            //    if (recip.Length > 2) recip = recip.Substring(0, recip.Length - 2);

            //    msg.To.Clear();
            //    msg.To.Add(new MailAddress(ConfigurationManager.AppSettings["EmailDebugUser"]));

            //    msg.Subject = msg.Subject + "[DEBUGGING]";
            //    msg.Body = "Original Recipients: " + recip + "\n\n" + msg.Body;
            //}
            //client.Send(msg);
        }
    }
}
