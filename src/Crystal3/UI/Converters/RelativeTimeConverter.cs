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
        }

        public string YearStringFormat { get; set; }
        public string MonthStringFormat { get; set; }
        public string DayStringFormat { get; set; }
        public string HourFormatString { get; set; }
        public string MinuteFormatString { get; set; }
        public string SecondFormatString { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = (DateTime)value;

            var timeDiff = DateTime.Now.Subtract(time);

            if (timeDiff.TotalDays > 365) return string.Format(YearStringFormat, Math.Round(timeDiff.TotalDays / 365));

            double months = Math.Round(timeDiff.TotalDays * 0.03285421);
            if (months >= 1) return string.Format(MonthStringFormat, months);

            if (timeDiff.TotalDays > 0) return string.Format(DayStringFormat, Math.Round(timeDiff.TotalDays));

            if (timeDiff.TotalHours > 0) return string.Format(HourFormatString, Math.Round(timeDiff.TotalHours));

            if (timeDiff.TotalMinutes > 0) return string.Format(MinuteFormatString, Math.Round(timeDiff.TotalMinutes));

            if (timeDiff.TotalSeconds > 0) return string.Format(SecondFormatString, Math.Round(timeDiff.TotalSeconds));

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
