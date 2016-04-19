using Log4Mongo;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Swallow.Service {

    public enum LogType {
        Fatal,
        Error,
        Warn,
        Debug
    }

    public sealed class LogService {

        public static void SetLog(MethodBase declaringType, Domain domain, LogType logtype, IList<string> parameters, string message = null) {
            ILog Log = LogManager.GetLogger(domain.ToString(), declaringType.DeclaringType + "|" + declaringType.Name);

            StringBuilder msg = new StringBuilder();
            foreach (var item in parameters)
                msg.AppendLine(item);
            msg.AppendLine(message);

            switch (logtype) {
                case LogType.Fatal:
                    Log.Fatal(msg.ToString());
                    break;
                case LogType.Error:
                    Log.Error(msg.ToString());
                    break;
                case LogType.Warn:
                    Log.Warn(msg.ToString());
                    break;
                case LogType.Debug:
                    Log.Debug(msg.ToString());
                    break;
                default:
                    break;
            }
        }

        private static LogService instance;
        public static LogService Instance {
            get { return instance ?? (instance = new LogService()); }
        }

        private static PatternLayout ZicLayout = new PatternLayout(Environment.NewLine + "时间：%date 线程ID：[%thread] 级别：%-5level 触发源：%logger property:[%property{NDC}] - " + Environment.NewLine + "Message：%message%newline");

        private static IDictionary<string, LevelRangeFilter> Filters = new Dictionary<string, LevelRangeFilter>();
        private static LevelRangeFilter FatalFilter = new LevelRangeFilter();
        private static LevelRangeFilter ErrorFilter = new LevelRangeFilter();
        private static LevelRangeFilter WarnFilter = new LevelRangeFilter();
        private static LevelRangeFilter InfoFilter = new LevelRangeFilter();
        private static LevelRangeFilter DebugFilter = new LevelRangeFilter();

        private static LevelRangeFilter EmailFilter = new LevelRangeFilter();

        private LogService() {
            FatalFilter.LevelMax = log4net.Core.Level.Fatal;
            FatalFilter.LevelMin = log4net.Core.Level.Fatal;
            FatalFilter.ActivateOptions();
            Filters.Add("FATAL", FatalFilter);

            ErrorFilter.LevelMax = log4net.Core.Level.Error;
            ErrorFilter.LevelMin = log4net.Core.Level.Error;
            ErrorFilter.ActivateOptions();
            Filters.Add("ERROR", ErrorFilter);

            WarnFilter.LevelMax = log4net.Core.Level.Warn;
            WarnFilter.LevelMin = log4net.Core.Level.Warn;
            WarnFilter.ActivateOptions();
            Filters.Add("WARN", WarnFilter);

            InfoFilter.LevelMax = log4net.Core.Level.Info;
            InfoFilter.LevelMin = log4net.Core.Level.Info;
            InfoFilter.ActivateOptions();
            Filters.Add("INFO", InfoFilter);

            DebugFilter.LevelMax = log4net.Core.Level.Debug;
            DebugFilter.LevelMin = log4net.Core.Level.Debug;
            DebugFilter.ActivateOptions();
            Filters.Add("DEBUG", DebugFilter);

            EmailFilter.LevelMax = Level.Fatal;
            EmailFilter.LevelMin = Level.Error;
            EmailFilter.ActivateOptions();
        }

        public void Config(string[] domains, IZicEmailConfig[] emailConfigs) {
            var mongodbConStr = ConfigurationManager.AppSettings["mongodbconstr"]; // attention!!
            foreach (string domain in domains) {
                var repository = LogManager.CreateRepository(domain);

                //FileLog
                foreach (var Filter in Filters) {
                    var fileAppender = new RollingFileAppender();
                    fileAppender.Name = domain + "_" + Filter.Key + "_FileAppender";
                    fileAppender.File = "Log_" + domain + "\\" + Filter.Key + "\\";
                    fileAppender.AppendToFile = true;
                    fileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
                    fileAppender.DatePattern = "yyyy-MM-dd'.log'";
                    fileAppender.StaticLogFileName = false;
                    fileAppender.Layout = ZicLayout;
                    fileAppender.AddFilter(Filter.Value);
                    fileAppender.ActivateOptions();
                    BasicConfigurator.Configure(repository, fileAppender);
                }

                // mongo
                if (!string.IsNullOrEmpty(mongodbConStr)) {
                    foreach (var filter in Filters) {
                        var mongoDBAppender = new MongoDBAppender();
                        mongoDBAppender.Name = domain + "_" + filter.Key + "_MongoDBAppender";
                        mongoDBAppender.ConnectionString = mongodbConStr;
                        mongoDBAppender.Layout = ZicLayout;
                        mongoDBAppender.AddFilter(filter.Value);
                        mongoDBAppender.ActivateOptions();
                        BasicConfigurator.Configure(repository, mongoDBAppender);
                    }
                }

                //SmtpLog
                //zic：邮件日志与其他日志同级别（甚至更高），所以不允许丢失或缓冲，所以Buffer、lossy、Evaluator全部忽略
                //如果缓冲区溢出在触发事件之前，日志事件可能会丢失。
                //如果log4net.Appender.BufferingAppenderSkeleton.Lossy设置为false防止日志事件被丢失。
                //如果log4net.Appender.BufferingAppenderSkeleton.Lossy设置为true，那么log4net.Appender.BufferingAppenderSkeleton.Evaluator必须被指定。
                //也就是说如果LevelEvaluator设为WARN，则在WARN或之上级别的日志肯定不会丢失，之下级别的日志有可能因为缓冲区溢出而丢失。
                foreach (var emailConfig in emailConfigs) {
                    var smtpAppender = new ZicSmtpAppender(emailConfig);
                    smtpAppender.Name = domain + "_SmtpAppender";
                    smtpAppender.Authentication = ZicSmtpAppender.SmtpAuthentication.Basic;
                    smtpAppender.Subject = "pig.ai " + domain + " logging message";
                    smtpAppender.BufferSize = 0;
                    smtpAppender.Lossy = false;
                    smtpAppender.Layout = ZicLayout;
                    smtpAppender.AddFilter(EmailFilter);
                    smtpAppender.ActivateOptions();
                    BasicConfigurator.Configure(repository, smtpAppender);
                }
            }
        }
    }

    public interface IZicEmailConfig {
        string To { get; set; }
        string From { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        bool EnableSSL { get; set; }
        string SmtpHost { get; set; }
        int Port { get; set; }
    }

    public class ZicGmailConfig : IZicEmailConfig {
        public string To { get; set; }
        public string From { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
        public ZicGmailConfig() {
            To = "zicjin@gmail.com";
            From = "zicjin@gmail.com";
            Username = "zicjin@gmail.com";
            Password = "*******";
            EnableSSL = true;
            SmtpHost = "smtp.gmail.com";
            Port = 587;
        }
    }

    public class SwallowConfig : IZicEmailConfig {
        public string To { get; set; }
        public string From { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSSL { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }

        public SwallowConfig(string mailTo) {
            To = "zicjin@gmail.com,zicjin@live.com";
            From = "cheney@qq.com";
            Username = "zicjin@qq.com";
            Password = "duantui";
            EnableSSL = true;
            SmtpHost = "smtp.exmail.qq.com";
            Port = 25;
        }
    }

    //Reference http://goo.gl/Bnwvs
    public class ZicSmtpAppender : BufferingAppenderSkeleton {

        public ZicSmtpAppender(IZicEmailConfig config) {
            To = config.To;
            From = config.From;
            Username = config.Username;
            Password = config.Password;
            EnableSSL = config.EnableSSL;
            SmtpHost = config.SmtpHost;
            Port = config.Port;
        }

        private string m_to;
        private string m_from;
        private string m_subject;
        private string m_smtpHost;
        private bool m_enableSSL;
        private string m_username;
        private string m_password;
        private int m_port = 25;
        private SmtpAuthentication m_authentication = SmtpAuthentication.None;
        private MailPriority m_mailPriority = MailPriority.Normal;

        public string To {
            get { return m_to; }
            set { m_to = value; }
        }

        public string From {
            get { return m_from; }
            set { m_from = value; }
        }

        public string Subject {
            get { return m_subject; }
            set { m_subject = value; }
        }

        public string SmtpHost {
            get { return m_smtpHost; }
            set { m_smtpHost = value; }
        }

        [Obsolete("Use the BufferingAppenderSkeleton Fix methods")]
        public bool LocationInfo {
            get { return false; }
            set {; }
        }

        public SmtpAuthentication Authentication {
            get { return m_authentication; }
            set { m_authentication = value; }
        }

        public string Username {
            get { return m_username; }
            set { m_username = value; }
        }

        public string Password {
            get { return m_password; }
            set { m_password = value; }
        }

        public int Port {
            get { return m_port; }
            set { m_port = value; }
        }

        public MailPriority Priority {
            get { return m_mailPriority; }
            set { m_mailPriority = value; }
        }

        public bool EnableSSL {
            get { return m_enableSSL; }
            set { m_enableSSL = value; }
        }

        protected override void SendBuffer(LoggingEvent[] events) {
            try {
#if !DEBUG
                StringWriter writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);

                string t = Layout.Header;
                if (t != null) {
                    writer.Write(t);
                }

                for (int i = 0; i < events.Length; i++) {
                    RenderLoggingEvent(writer, events[i]);
                }

                t = Layout.Footer;
                if (t != null) {
                    writer.Write(t);
                }

                SmtpClient smtpClient = new SmtpClient(m_smtpHost, m_port);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = m_enableSSL;

                if (m_authentication == SmtpAuthentication.Basic) {
                    smtpClient.Credentials = new NetworkCredential(m_username, m_password);
                } else if (m_authentication == SmtpAuthentication.Ntlm) {
                    smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                if (events.Length > 0) {
                    var exceptionObject = events[0].ExceptionObject;
                    if (exceptionObject != null) {
                        this.m_subject += exceptionObject.Message;
                    }
                }
                MailMessage mailMessage = new MailMessage(m_from, m_to, m_subject, writer.ToString());
                mailMessage.Priority = m_mailPriority;
                smtpClient.Send(mailMessage);
#endif
            } catch (Exception e) {
                ErrorHandler.Error("Error occurred while sending e-mail notification.", e);
            }
        }

        protected override bool RequiresLayout {
            get { return true; }
        }

        public enum SmtpAuthentication {
            None,
            Basic,
            Ntlm
        }
    }
}
