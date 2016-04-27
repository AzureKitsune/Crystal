using Crystal3.InversionOfControl;
using Crystal3.Model;
using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static ViewModelBase GetViewModel(this Page page)
        {
            return page.DataContext as ViewModelBase;
        }

        public static Type GetNavigationViewModelType(this Page page)
        {
            TypeInfo pageTypeInfo = page.GetType().GetTypeInfo();

            if (pageTypeInfo.CustomAttributes.Any(y => y.AttributeType == typeof(NavigationViewModelAttribute)))
            {
                var linkAttribute = pageTypeInfo.CustomAttributes.First(y => y.AttributeType == typeof(NavigationViewModelAttribute));

                var viewModelType = (Type)linkAttribute.ConstructorArguments.FirstOrDefault(x => ((Type)x.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).Value;
                if (viewModelType == null)
                    viewModelType = (Type)linkAttribute.NamedArguments.First(x => ((Type)x.TypedValue.Value).GetTypeInfo().IsSubclassOf(typeof(ViewModelBase))).TypedValue.Value;

                return viewModelType;
            }

            return null;
        }

        public static IoCContainer GetNavigationViewModelIoCContainer(this Page page)
        {
            var viewModelType = GetNavigationViewModelType(page);
            if (viewModelType != null)
            {
                return IoC.GetContainerForViewModel(viewModelType);
            }

            return null;
        }
    }
}
