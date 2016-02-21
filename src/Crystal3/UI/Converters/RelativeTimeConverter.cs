using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Crystal3.UI.Converters
{
    public class RelativeTimeConverter : IValueConverter
    {
        public RelativeTimeConverter()
        {
            YearStringFormat = "{0} years ago";
            MonthStringFormat = "{0} months ago";
            DayStringFormat = "{0} day(s) ago";
            HourFormatString = "{0} hours ago";
            MinuteFormatString = "{0} minutes ago";
            SecondFormatString = "{0} seconds ago";

            DayAndMonthStringFormat = "{0} months, and {1} days ago";

            NegativeTimeDiffErrorStringFormat = "In Another Universe";
        }

        public string YearStringFormat { get; set; }
        public string MonthStringFormat { get; set; }
        public string DayStringFormat { get; set; }
        public string HourFormatString { get; set; }
        public string MinuteFormatString { get; set; }
        public string SecondFormatString { get; set; }

        public bool CombineDaysAndMonths { get; set; }
        public string DayAndMonthStringFormat { get; set; }

        public string NegativeTimeDiffErrorStringFormat { get; set; }

        public RelativeTimeConverterMode TimeMode { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return NegativeTimeDiffErrorStringFormat;

            DateTime time;
            if (value is DateTime)
            {
                time = (DateTime)value;
            }
            else if (value is string && !string.IsNullOrWhiteSpace(value as string))
            {
                if (!DateTime.TryParse(value as string, out time))
                    return NegativeTimeDiffErrorStringFormat;
            }
            else
                return NegativeTimeDiffErrorStringFormat;

            var timeDirection = parameter ?? Enum.GetName(typeof(RelativeTimeConverterMode), TimeMode).ToLower();

            TimeSpan timeDiff;

            if (((string)timeDirection).ToLower() == "future")
            {
                timeDiff = time.Subtract(DateTime.Now);
            }
            else
            {
                timeDiff = DateTime.Now.Subtract(time);
            }

            if (timeDiff.TotalDays > 365) return string.Format(YearStringFormat, Math.Round(timeDiff.TotalDays / 365));

            double months = Math.Round(timeDiff.TotalDays * 0.03285421);
            if (months >= 1)
            {
                if (!CombineDaysAndMonths)
                    return string.Format(MonthStringFormat, months);
                else
                    return string.Format(DayAndMonthStringFormat, months, Math.Round(timeDiff.TotalDays));
            }

            if (timeDiff.TotalDays >= 1) return string.Format(DayStringFormat, Math.Round(timeDiff.TotalDays));

            if (timeDiff.TotalHours >= 1) return string.Format(HourFormatString, Math.Round(timeDiff.TotalHours));

            if (timeDiff.TotalMinutes > 0) return string.Format(MinuteFormatString, Math.Round(timeDiff.TotalMinutes));

            if (timeDiff.TotalSeconds > 0) return string.Format(SecondFormatString, Math.Round(timeDiff.TotalSeconds));

            if (timeDiff.TotalMilliseconds < 0) return NegativeTimeDiffErrorStringFormat;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public enum RelativeTimeConverterMode
    {
        Past = 0,
        Future = 1
    }
}
