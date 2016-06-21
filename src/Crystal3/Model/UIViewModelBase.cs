using Crystal3.Navigation;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Crystal3.Model
{
    /// <summary>
    /// A view model base class with UI sugar added on top.
    /// </summary>
    public abstract class UIViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Creates a new UIViewModelBase
        /// </summary>
        public UIViewModelBase()
        {
            Status = WindowManager.GetStatusManagerForCurrentWindow();

            UI = new UIViewModelBaseUIWrapper();
        }

        /// <summary>
        /// Returns whether the View Model is busy.
        /// </summary>
        public bool IsBusy { get { return GetPropertyValue<bool>(); } protected set { SetPropertyValue<bool>(value: value); } }

        /// <summary>
        /// Returns the StatusManager instance for this View Model.
        /// </summary>
        public StatusManager Status { get; private set; }

        protected internal override void OnNavigatedFrom(object sender, CrystalNavigationEventArgs e)
        {
            //Clean up!
            Status.RemoveAllControllersForCallingViewModel(this);

            UI.Cleanup();

            base.OnNavigatedFrom(sender, e);
        }

        public UIViewModelBaseUIWrapper UI { get; private set; }

        public class UIViewModelBaseUIWrapper
        {
            private FrameworkElement uiElement = null;
            internal UIViewModelBaseUIWrapper()
            {

            }

            internal void Cleanup()
            {

            }

            internal void SetUIElement(FrameworkElement element)
            {
                uiElement = element;
            }

            public Task WaitForUILoadAsync()
            {
                if (uiElement == null) return Task.CompletedTask; //todo may have to change this.

                //todo add a check to see if it is loaded via reflection

                var contentLoadedField = uiElement.GetType().GetField("_contentLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
                if (contentLoadedField != null)
                {
                    if ((bool)contentLoadedField.GetValue(uiElement))
                    {
                        return Task.CompletedTask;
                    }
                }

                TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();

                RoutedEventHandler loadedHandler = null;
                loadedHandler = new RoutedEventHandler((object sender, RoutedEventArgs args) =>
                {
                    uiElement.Loaded -= loadedHandler;

                    taskSource.SetResult(null);
                });

                uiElement.Loaded += loadedHandler;

                return taskSource.Task;
            }
        }
    }
}
