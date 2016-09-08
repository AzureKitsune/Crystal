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
    public static class UIViewModelFragmentShim
    {
        #region HandleVisibilityChange
        private static readonly DependencyProperty HandleVisibilityChange_HandlerProperty = DependencyProperty.RegisterAttached(
            "HandleVisibilityChange_Handler", typeof(long), typeof(Grid), new PropertyMetadata(false));
        public static readonly DependencyProperty HandleVisibilityChangeProperty = DependencyProperty.RegisterAttached(
            "HandleVisibilityChange", typeof(bool), typeof(Grid), new PropertyMetadata(false));

        public static void SetHandleVisibilityChange(DependencyObject element, bool value)
        {
            element.SetValue(HandleVisibilityChangeProperty, value);

            Grid control = element as Grid;
            ViewModelFragment fragment = control.DataContext as ViewModelFragment;

            if (value)
            {
                var key = element.RegisterPropertyChangedCallback(UIElement.VisibilityProperty, new DependencyPropertyChangedCallback((DependencyObject cont, DependencyProperty proper) =>
                {
                    fragment.OnVisibilityChanged((Visibility)cont.GetValue(proper), null);
                }));

                element.SetValue(HandleVisibilityChange_HandlerProperty, key);
            }
            else
            {
                var key = (long)element.GetValue(HandleVisibilityChange_HandlerProperty);
                element.UnregisterPropertyChangedCallback(UIElement.VisibilityProperty, key);
            }
        }

        public static bool GetHandleVisibilityChange(DependencyObject element)
        {
            return (bool)element.GetValue(HandleVisibilityChangeProperty);
        }
        #endregion
    }
}
