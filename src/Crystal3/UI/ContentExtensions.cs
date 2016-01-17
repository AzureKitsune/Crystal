using Crystal3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3.UI
{
    public static class ContentExtensions
    {
        #region Fake Navigation On ViewModel

        public static readonly DependencyProperty FakeNavigationOnViewModelProperty =
            DependencyProperty.RegisterAttached("FakeNavigationOnViewModel",
            typeof(bool), typeof(ContentControl), new PropertyMetadata(""));


        public static void SetFakeNavigationOnViewModel(ContentControl element, bool value)
        {
            if (value)
                element.DataContextChanged += Element_DataContextChanged;
            else
                element.DataContextChanged -= Element_DataContextChanged;

            element.SetValue(FakeNavigationOnViewModelProperty, value);
        }

        private static void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                if (sender.DataContext != null)
                    if (sender.DataContext is ViewModelBase)
                        ((ViewModelBase)sender.DataContext).OnNavigatedFrom(sender, new Navigation.CrystalNavigationEventArgs() { Direction = Navigation.CrystalNavigationDirection.Refresh });

                if (args.NewValue is ViewModelBase)
                    ((ViewModelBase)args.NewValue).OnNavigatedTo(sender, new Navigation.CrystalNavigationEventArgs() { Direction = Navigation.CrystalNavigationDirection.Refresh });
            }
        }

        public static bool GetFakeNavigationOnViewModel(ContentControl element)
        {
            return (bool)element.GetValue(FakeNavigationOnViewModelProperty);
        }
        #endregion
    }
}
