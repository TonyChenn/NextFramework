using System.Text;

namespace NextFramework.Util
{
    public class TimeHelper
    {
        public static string GetTimeString(int seconds)
        {
            int day = seconds / 86400;
            seconds %= 86400;
            int hour = seconds / 3600;
            seconds %= 3600;
            int minutes = seconds / 60;

            StringBuilder builder = new StringBuilder();
            if (day > 0)
                builder.Append(day + "天 ");
            if (hour > 0)
                builder.Append(hour + "小时");
            if (minutes > 0)
                builder.Append(minutes + "分钟");
            builder.Append(seconds);

            return builder.ToString();
        }
    }
}

