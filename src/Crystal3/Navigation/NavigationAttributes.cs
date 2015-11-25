using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Navigation
{
    public class NavigationAttributes : DependencyObject
    {
        #region PageTitle
        /// <summary>
        /// Dependency property to set the page title for navigation.
        /// </summary>
        public static readonly DependencyProperty PageTitleProperty =
            DependencyProperty.RegisterAttached("PageTitle",
            typeof(string), typeof(Page), new PropertyMetadata(""));

        /// <summary>
        /// Sets the page title.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPageTitle(UIElement element, string value)
        {
            element.SetValue(PageTitleProperty, value);
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetPageTitle(UIElement element)
        {
            return (string)element.GetValue(PageTitleProperty);
        }
        #endregion

        #region Navigation Hint

        public static readonly DependencyProperty NavigationHintProperty =
            DependencyProperty.RegisterAttached("NavigationHint",
            typeof(string), typeof(UIElement), new PropertyMetadata(""));


        public static void SetNavigationHint(UIElement element, string value)
        {
            if (CrystalApplication.Current.GetType().GetTypeInfo().Assembly.DefinedTypes.FirstOrDefault(x => x.FullName == value) == null)
                throw new ArgumentException("Type not found.", "value");

            element.SetValue(NavigationHintProperty, value);
        }


        public static string GetNavigationHint(UIElement element)
        {
            return (string)element.GetValue(NavigationHintProperty);
        }
        #endregion
    }
}
