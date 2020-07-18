﻿using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace fsrhilmakv2.Extra
{
    public class EmailHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static void sendEmail(List<string> to, string subject, string body)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            m.From = new MailAddress("info@arfashion-u.com");
            foreach (var item in to)
            {
                m.To.Add(item);
            }
            m.Subject = subject;
            m.Body = body;
            sc.Host = "mail5008.site4now.net";
            string str1 = "gmail.com";
            string str2 = "info@arfashion-u.com";
            if (str2.Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential("info@arfashion-u.com", "822357kenan$");
                    sc.EnableSsl = true;
                    sc.Send(m);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                try
                {
                    sc.Port = 25;
                    sc.Credentials = new System.Net.NetworkCredential("info@arfashion-u.com", "822357kenan$");
                    sc.EnableSsl = false;
                    sc.Send(m);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            EmailLog log = new EmailLog();
            log.CreationDate = DateTime.Now;
            log.LastModificationDate = DateTime.Now;
            log.Subject = subject;
            log.Body = body;
            log.Sender = "info@arfashion-u.com";
            log.Receiver = string.Join(";", to.ToArray());
            db.EmailLogs.Add(log);
            db.SaveChanges();

        }

        public static List<string> getReceivers(string s)
        {
            return s.Split(new char[] { ';' }).ToList();
        }
    }
}