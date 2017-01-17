using log4net;
using System;

namespace CSharpHelper
{
    public static class LogHelper
    {
        public static void ErrorWriteLog(string msg)
        {
            ILog logger = LogManager.GetLogger("logerror");
            logger.Error(msg);
        }

        public static void InfoWriteLog(string msg)
        {
            ILog logger = LogManager.GetLogger("loginfo");
            logger.Info(msg);
        }
    }
}
