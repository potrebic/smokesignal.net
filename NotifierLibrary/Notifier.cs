// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace NotifierLibrary
{
    /// <summary>
    /// Interface for sending notification. Needed so that test code can have a mock implementation
    /// </summary>
    public interface INotifier
    {
        void Doit(string userName, string roomName, int roomId, string emailAddr, string campfireName, string requesterName, string context);
        void SendHelp(string userName, string emailAddr);
        void SendSettings(CampfireState.UserInfo user, bool haveChanged); 
        void SendWelcomeEmail(string userName, string emailAddr, string extraMessage);
    }

    public class Notifier : INotifier
    {
        private SmtpClient ConfigSmtpClient()
        {
            SmtpClient smtp = new SmtpClient();
            return smtp;
        }

        private void AppendWithNewLine(StringBuilder sb, string str)
        {
            sb.Append(str);
            sb.Append("\n");
        }

        private void AppendWithNewLine(StringBuilder sb)
        {
            sb.Append("\n");
        }

        public void SentTestEmail(string toAddr, string fromAddr, string host, int port, string credsUserName, string credsPassword, bool defaultCreds, string extraMessage)
        {
            MailMessage msg = new MailMessage();
            if (!string.IsNullOrEmpty(fromAddr))
            {
                msg.From = new MailAddress(fromAddr);
            }

            msg.To.Add(new MailAddress(toAddr));

            SmtpClient smtp = new SmtpClient(host);
            smtp.UseDefaultCredentials = defaultCreds;
            if (!smtp.UseDefaultCredentials && !string.IsNullOrEmpty(credsUserName))
            {
                smtp.Credentials = new System.Net.NetworkCredential(credsUserName, credsPassword);
            }

            if (port != 0)
            {
                smtp.Port = port;
            }

            msg.BodyEncoding = Encoding.ASCII;
            msg.Subject = "Test email from Smoke Signal Startup";

            msg.Body = ComposeWelcomeEmailBody(extraMessage);
//            msg.Body = "Here's the email to confirm that your email settings in Smoke Signal Startup are working.";

            smtp.Send(msg);
        }

        #region INotifier Members

        public void SendSettings(CampfireState.UserInfo user, bool haveChanged)
        {
            if (user == null) return;

            try
            {
                MailMessage msg = new MailMessage();

                msg.To.Add(new MailAddress(user.EmailAddress));
                msg.BodyEncoding = Encoding.ASCII;
                msg.Subject = string.Format(haveChanged ? "Your Smoke Signal settings have changed..." : "Your Smoke Signal settings");
                StringBuilder sb = new StringBuilder();

                AppendWithNewLine(sb, string.Format("Dear {0}", user.NickName));
                AppendWithNewLine(sb);

                AppendWithNewLine(sb, string.Format(haveChanged
                    ? "Your SmokeSignal settings have been changed per your request:"
                    : "Here are the Smoke Signal settings as requested:"));

                int delay = user.DelayInMinutes > 0 ? user.DelayInMinutes : SmokeSignalConfig.Instance.DelayBeforeSmokeSignalInMinutes;
                bool defaultDelay = user.DelayInMinutes < 1;

                AppendWithNewLine(sb, string.Format("    Campfire Email     : {0}", user.EmailAddress));
                AppendWithNewLine(sb, string.Format("    Smoke Signal Email : {0}", user.SmokeEmail));
                AppendWithNewLine(sb, string.Format("    Delay In Minutes   : {0}{1}", delay, defaultDelay ? " (default)" : ""));

                AppendWithNewLine(sb);
                AppendWithNewLine(sb, "Sincerely,");
                AppendWithNewLine(sb, "Smoke Signal");

                msg.Body = sb.ToString();

                SmtpClient smtp = ConfigSmtpClient();

                Utils.TraceInfoMessage(string.Format("Sending Settings email. {0}", user.EmailAddress));

                smtp.Send(msg);
            }
            catch (InvalidOperationException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
            catch (SmtpException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
        }

        private string ComposeWelcomeEmailBody(string extraMessage)
        {
            StringBuilder sb = new StringBuilder();

            AppendWithNewLine(sb, "Welcome,");
            AppendWithNewLine(sb, "");
            AppendWithNewLine(sb, "This campfire is using the SmokeSignal addon. Enter the following message in any room to learn more:");
            AppendWithNewLine(sb, "    @SmokeSignal:Help");
            if (!string.IsNullOrEmpty(extraMessage))
            {
                AppendWithNewLine(sb, "");
                AppendWithNewLine(sb, extraMessage);
            }
            AppendWithNewLine(sb);
            AppendWithNewLine(sb, "Sincerely,");
            AppendWithNewLine(sb, "Smoke Signal");
            return sb.ToString();
        }
        public void SendWelcomeEmail(string userName, string emailAddr, string extraMessage)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(emailAddr));
                msg.BodyEncoding = Encoding.ASCII;
                msg.Subject = string.Format("Welcome to campfire w/ smokesignal...");

                msg.Body = ComposeWelcomeEmailBody(extraMessage);
                SmtpClient smtp = ConfigSmtpClient();
                Utils.TraceInfoMessage(string.Format("Sending Help email. {0}", emailAddr));
                smtp.Send(msg);
            }
            catch (InvalidOperationException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
            catch (SmtpException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
        }
        public void SendHelp(string userName, string emailAddr)
        {
            try
            {
                MailMessage msg = new MailMessage();

                msg.To.Add(new MailAddress(emailAddr));
                msg.BodyEncoding = Encoding.ASCII;
                msg.Subject = string.Format("Smoke Signal help...");

                StringBuilder sb = new StringBuilder();

                AppendWithNewLine(sb, "Here's a mini Smoke Signal tutorial:");
                AppendWithNewLine(sb, "- To send a Smoke Signal to Jane Doe simply call her out in a campfire message like:");
                AppendWithNewLine(sb, "      Hey, @JaneD, do you know when the shipment of oranges will arrive?");
                AppendWithNewLine(sb, "  If Jane doesn't respond within a few minutes, she'll be sent an email.");
                AppendWithNewLine(sb, "- You can call out a person using any combo of First/Last Name, either entire name of single letter initial");
                AppendWithNewLine(sb, "     @Jane, @Doe, @JaneDoe, @JDoe, @JaneD");
                AppendWithNewLine(sb, "  The tag used must uniquely identify a user registered to your campfire, and be at least 3 letters long.");
                AppendWithNewLine(sb, "- To trigger an immediate email use '!' instead of '@' (e.g. !Jane)");
                AppendWithNewLine(sb, "- You can tweak your Smoke Signal config by interacting with the Smoke Signal service:");
                AppendWithNewLine(sb, "    - '@SmokeSignal:Help' generate this email");
                AppendWithNewLine(sb, "    - '@SmokeSignal:Settings' sends you an email listing your configuration");
                AppendWithNewLine(sb, "    - '@SmokeSignal:Delay=N' set a N minute delay before YOU ARE sent smoke signal emails");
                AppendWithNewLine(sb, "              valid values are 1 ... 10");
                AppendWithNewLine(sb, "              -1 will reset to the company default");
                AppendWithNewLine(sb, "    - '@SmokeSignal:AltEmail=someone@foop.com' use this email for Smoke Signals");
                AppendWithNewLine(sb, "    - '@SmokeSignal:AltEmail=' reset the Smoke Signal email to your campfire email address");
                AppendWithNewLine(sb, "  Just type these commands into any campfire room. You'll  receive a confirmation email to document");
                AppendWithNewLine(sb, "  any changes. Confirmation emails ALWAYS go to your campfire email address, not any 'AltEmail' address.");

                AppendWithNewLine(sb);
                AppendWithNewLine(sb, "Sincerely,");
                AppendWithNewLine(sb, "Smoke Signal");

                msg.Body = sb.ToString();

                SmtpClient smtp = ConfigSmtpClient();
                Utils.TraceInfoMessage(string.Format("Sending Help email. {0}", emailAddr));

                smtp.Send(msg);
            }
            catch (InvalidOperationException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
            catch (SmtpException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a help email");
            }
        }

        public void Doit(string userName, string roomName, int roomId, string emailAddr, string campfireName,
            string requesterName, string context)
        {
            try
            {
                bool shortForm = true;
                string targetAddr = emailAddr;
                MailMessage msg = new MailMessage();

                msg.To.Add(new MailAddress(targetAddr));

                msg.BodyEncoding = Encoding.ASCII;

                msg.Subject = shortForm ? 
                    string.Format("Smoke Signal - room \"{1}\", from {0}", requesterName, roomName) :
                    string.Format("{0} is sending you a Smoke Signal from \"{1}\" in Campfire", requesterName, roomName);

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("https://{0}.campfirenow.com/room/{1}", campfireName, roomId.ToString());
                AppendWithNewLine(sb);
                if (!string.IsNullOrEmpty(context))
                {
                    sb.AppendFormat("{0}", context);
                    AppendWithNewLine(sb);
                }

                //if (!shortForm)
                {
                    AppendWithNewLine(sb);
                    AppendWithNewLine(sb, "Sincerely,");
                    AppendWithNewLine(sb, "Smoke Signal");
                }

                msg.Body = sb.ToString();

                SmtpClient smtp = ConfigSmtpClient();

                Utils.TraceInfoMessage(string.Format("Sending email. {0}", targetAddr));

                smtp.Send(msg);
            }
            catch (InvalidOperationException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a notification email");
            }
            catch (SmtpException ex)
            {
                Utils.TraceException(TraceLevel.Error, ex, "Exception trying to send a notification email");
            }
        }

        #endregion
    }
}
