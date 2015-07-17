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
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = (DateTime)value;

            var timeDiff = DateTime.Now.Subtract(time);

            if (timeDiff.TotalDays > 365) return Math.Round(timeDiff.TotalDays / 365) + " years ago";

            double months = Math.Round(timeDiff.TotalDays * 0.03285421);
            if (months >= 1) return months + " months ago";

            if (timeDiff.TotalDays > 0) return timeDiff.TotalDays + " day(s) ago";

            if (timeDiff.TotalHours > 0) return timeDiff.TotalHours + " hours ago";

            if (timeDiff.TotalMinutes > 0) return timeDiff.TotalMinutes + " minutes ago";

            if (timeDiff.TotalSeconds > 0) return timeDiff.TotalSeconds + " seconds ago";

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
