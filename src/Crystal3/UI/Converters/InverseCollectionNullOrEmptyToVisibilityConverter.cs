using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Crystal3.UI.Converters
{
    public class InverseCollectionNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var returnValue = ((Visibility)new CollectionNullOrEmptyToVisibilityConverter().Convert(value, targetType, parameter, language)) == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
