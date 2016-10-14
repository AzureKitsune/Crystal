using Crystal3.Model;
using Crystal3.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Crystal3.UI
{
    public sealed partial class FragmentContentViewer : UserControl
    {
        public FragmentContentViewer()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty AutoInvokeProperty = DependencyProperty.Register("AutoInvoke", typeof(bool), typeof(FragmentContentViewer), new PropertyMetadata(true));
        public bool AutoInvoke
        {
            get { return (bool)GetValue(AutoInvokeProperty); }
            set { SetValue(AutoInvokeProperty, value); }
        }

        public static readonly DependencyProperty InvokeParameterProperty = DependencyProperty.Register("InvokeParameter", typeof(object), typeof(FragmentContentViewer), new PropertyMetadata(false));
        public object InvokeParameter
        {
            get { return GetValue(InvokeParameterProperty); }
            set { SetValue(InvokeParameterProperty, value); }
        }

        public static readonly DependencyProperty FragmentProperty = DependencyProperty.Register("Fragment", typeof(ViewModelFragment), typeof(FragmentContentViewer), new PropertyMetadata(null, HandleFragmentPropertyChanged));

        public ViewModelFragment Fragment
        {
            get { return (ViewModelFragment)GetValue(FragmentProperty); }
            set { SetValue(FragmentProperty, value); }
        }

        private static void HandleFragmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as FragmentContentViewer;
            if (viewer != null)
            {
                HandleDisplay(viewer, e.OldValue as ViewModelFragment, e.NewValue as ViewModelFragment);
            }
        }

        private static void HandleDisplay(FragmentContentViewer viewer, ViewModelFragment oldValue, ViewModelFragment newValue)
        {
            if (oldValue != null)
                oldValue.Dispose();

            if (newValue != null)
            {
                var viewType = FragmentManager.ResolveFragmentView(newValue.GetType());

                if (viewType != null)
                {
                    var view = Activator.CreateInstance(viewType) as FrameworkElement;

                    var newFragment = newValue as ViewModelFragment;

                    view.DataContext = newFragment;
                    newFragment.UI.SetUIElement(view);

                    viewer.PART_ContentPresenter.Content = view;

                    newFragment.Invoke(viewer.DataContext as ViewModelBase, null);
                }
            }
            else
            {
                viewer.PART_ContentPresenter.Content = null;
            }
        }
    }
}
