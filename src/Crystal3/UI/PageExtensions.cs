using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Crystal3.UI
{
    public static class PageExtensions
    {
        public static void BindPageTitleToViewModelProperty(this Page page, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            page.SetBinding(NavigationAttributes.PageTitleProperty, new Windows.UI.Xaml.Data.Binding()
            {
                Path = new PropertyPath(propertyName),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = page.GetValue(NavigationAttributes.PageTitleProperty) as string
            });
        }
    }
}
