using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpHelperLib.ProcessResultInfo;
using CSharpHelperLib.TarAndZip;
using RemoteSenseCommLib.Tools;
namespace CSharpHelperTest
{
    class Program
    {
        static void Main(string[] args)
        {
          //  TestLogHelper();
            //TestDateTimeHelper();
            TestTarAndZip();

        }

        private static void TestTarAndZip()
        {
           //TarHelper helper = new TarHelper();
            //helper.ExtractTar(@"E:\test\HDFExplorer.rar", @"e:\test\b");
         //WinRarHelper helper = new WinRarHelper();
         ProcessResultInfo processResultInfo= new ProcessResultInfo();
            WinRarHelper.DeCompressRar(@"E:\test\a.rar", @"e:\test\b",out processResultInfo);
        }

        private static void TestDateTimeHelper()
        {
     
    
                //这里先打印下我的时间吧
                if (true)
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-15 11:38:56:7364468
                }

                //获取年的开始时间和结束时间
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfYear(2015);
                    DateTime end = DateTimeHelper.GetEndOfYear(2015);
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2015-01-01 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2015-12-31 23:59:59:9990000
                }
                //获取当前年的开始日期和结束日期
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfCurrentYear();
                    DateTime end = DateTimeHelper.GetEndOfCurrentYear();
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-01-01 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-12-31 23:59:59:9990000
                }
                //是否是闰年和年的天数
                if (true)
                {
                    bool b2015 = DateTimeHelper.IsLeapOfYear(2015);
                    bool b2016 = DateTimeHelper.IsLeapOfCurrentYear();
                    Console.WriteLine("2015闰年？{0}", b2015);//2015闰年？False
                    Console.WriteLine("2016闰年？{0}", b2016);//2016闰年？True
                    int days2015 = DateTimeHelper.GetTotalDaysOfYear(new DateTime(2015, 1, 3));
                    int days2016 = DateTimeHelper.GetTotalDaysOfYear(2016);
                    Console.WriteLine("2015多少天：{0}", days2015);//2015多少天：365
                    Console.WriteLine("2016多少天：{0}", days2016);//2016多少天：366
                }
                //季度相关的
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfQuarter(2016, 1);
                    DateTime end = DateTimeHelper.GetEndOfQuarter(2016, 1);
                    int quarter = DateTimeHelper.GetQuarter(12);
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));// 2016-01-01 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-03-31 23:59:59:9990000
                    Console.WriteLine(quarter);//4
                }
                //月相关的
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfCurrentMonth();
                    DateTime end = DateTimeHelper.GetEndOfMonth(DateTime.Now);
                    int days = DateTimeHelper.GetTotalDayOfMonth(DateTime.Now);
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-01 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-30 23:59:59:9990000
                    Console.WriteLine(days);//30
                }
                //旬相关的
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfCurrentTen();
                    DateTime end = DateTimeHelper.GetEndOfTenFirst(2016, 12);
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-11 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-12-10 23:59:59:9990000
                }
                //周相关
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfWeek(DateTime.Now);
                    DateTime end = DateTimeHelper.GetEndOfWeek(DateTime.Now);
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-13 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-19 23:59:59:9990000
                }
                //日相关
                if (true)
                {
                    DateTime start = DateTimeHelper.GetStartOfCurrentDay();
                    DateTime end = DateTimeHelper.GetEndOfCurrentDay();
                    Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-15 00:00:00:0000000
                    Console.WriteLine(end.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-11-15 23:59:59:9990000
                }
                //格式控制
                if (true)
                {
                    DateTime dt = DateTime.Now;
                    string s1 = DateTimeHelper.GetDateString(dt);
                    string s2 = DateTimeHelper.GetDateTimeString(dt);
                    string s3 = DateTimeHelper.GetDateTimeFullString(dt);
                    Console.WriteLine(s1);//2016-11-15
                    Console.WriteLine(s2);//2016-11-15 13:13:12
                    Console.WriteLine(s3);//2016-11-15 13:13:12:9846102
                }
                //日期比较的
                if (true)
                {
                    DateTime dt1 = DateTime.Now;
                    DateTime dt2 = new DateTime(2016, 2, 3);
                    DateTime dt3 = DateTime.Now.AddDays(-1).AddMinutes(2).AddSeconds(3).AddMilliseconds(200);
                    DateTime dt4 = DateTime.Parse("2016-07-12");
                    List<DateTime> list = new List<DateTime>() { dt1, dt2, dt3, dt4 };
                    DateTime max = DateTimeHelper.GetMax(list.ToArray());
                    DateTime min = DateTimeHelper.GetMin(list.ToArray());
                    Console.WriteLine("dt1={0}\ndt2={1}\ndt3={2}\ndt4={3}\n",
                        DateTimeHelper.GetDateTimeString(dt1),
                        DateTimeHelper.GetDateTimeString(dt2),
                    DateTimeHelper.GetDateTimeString(dt3),
                    DateTimeHelper.GetDateTimeString(dt4));
                    Console.WriteLine("max=" + DateTimeHelper.GetDateTimeString(max));
                    Console.WriteLine("min=" + DateTimeHelper.GetDateTimeString(min));
                    //dt1=2016-11-15 13:26:14
                    //dt2=2016-02-03 00:00:00
                    //dt3=2016-11-14 13:28:18
                    //dt4=2016-07-12 00:00:00
                    //
                    //max=2016-11-15 13:26:14
                    //min=2016-02-03 00:00:00

                    max = DateTimeHelper.GetMax(new DateTime(2016, 1, 2), new DateTime(2016, 1, 3));
                    Console.WriteLine("max=" + DateTimeHelper.GetDateTimeString(max));//max=2016-01-03 00:00:00
                }
                //获取日期的差值  
                if (true)
                {
                    DateTime dt1 = new DateTime(2016, 12, 3, 20, 18, 19);
                    DateTime dt2 = new DateTime(2016, 12, 2, 8, 10, 10);
                    TimeSpan timeSpan = DateTimeHelper.GetDifference(dt1, dt2);
                    Console.WriteLine("dt1-dt2={0}天{1}小时{2}分钟{3}秒{4}毫秒", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                    //dt1-dt2=1天12小时8分钟9秒0毫秒
                    double days = DateTimeHelper.GetDifferenceOfDay(dt1, dt2);
                    double hours = DateTimeHelper.GetDifferenceOfHours(dt1, dt2);
                    double minutes = DateTimeHelper.GetDifferenceOfMinutes(dt1, dt2);
                    double seconds = DateTimeHelper.GetDifferenceOfSeconds(dt1, dt2);

                    Console.WriteLine("dt1-dt2={0}days", days);
                    Console.WriteLine("dt1-dt2={0}hours", hours);
                    Console.WriteLine("dt1-dt2={0}minutes", minutes);
                    Console.WriteLine("dt1-dt2={0}seconds", seconds);
                    //dt1-dt2=1.50565972222222days
                    //dt1-dt2=36.1358333333333hours
                    //dt1-dt2=2168.15minutes
                    //dt1-dt2=130089seconds
                }
                //解析日期扩展
                if (true)
                {
                    DateTime dt1 = DateTime.Parse("2016-10-12");
                    DateTime dt2 = DateTimeHelper.ParseExt("2016年10月12日");
                    Console.WriteLine(dt1.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-10-12 00:00:00:0000000
                    Console.WriteLine(dt2.ToString("yyyy-MM-dd HH:mm:ss:fffffff"));//2016-10-12 00:00:00:0000000
                }
         }

        private static void TestLogHelper()
        {
            LogHelper.WriteLog(LogInfoType.Message, "这里只是一个测试");
        }
    }
}
