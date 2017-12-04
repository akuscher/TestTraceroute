using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestTraceroute
{
    public class Utils
    {
        public static log4net.Appender.IAppender CreateFileAppender(string name, string fileName)
        {


            log4net.Appender.FileAppender appender = new
            log4net.Appender.FileAppender();
            appender.Name = name;
            appender.File = fileName;
            appender.AppendToFile = true;

            log4net.Layout.PatternLayout layout = new
            log4net.Layout.PatternLayout();
            //layout.ConversionPattern = "%d [%t] %-5p %c [%x] - %m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        // Add an appender to a logger
        public static void AddAppender(string loggerName, log4net.Appender.IAppender appender)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(loggerName);
            log4net.Repository.Hierarchy.Logger l =
                 (log4net.Repository.Hierarchy.Logger)log.Logger;

            l.AddAppender(appender);

        }

    }
}
