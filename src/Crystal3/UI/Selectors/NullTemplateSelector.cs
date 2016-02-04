using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3.UI.Selectors
{
    public class NullTemplateSelector: DataTemplateSelector
    {
        public DataTemplate ObjectNullTemplate { get; set; }
        public DataTemplate ObjectTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item == null || ((item is string) && string.IsNullOrWhiteSpace(item.ToString())))
                return ObjectNullTemplate;
            else
                return ObjectTemplate;
        }
    }
}
