using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Crystal3.UI.Converters
{
    //https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
    public class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is DateTime)
            {
                var dateTime = (DateTime)value;

                return dateTime.ToString("t");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
