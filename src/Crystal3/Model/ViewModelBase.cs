using Crystal3.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Crystal3.Navigation;
using System.Runtime.CompilerServices;
using Crystal3.Core;
using Crystal3.InversionOfControl;

namespace Crystal3.Model
{
    /// <summary>
    /// A base class for ViewModels to inherit which implements INotifyPropertyChanged.
    /// </summary>
    public abstract partial class ViewModelBase: INotifyPropertyChanged
    {
        /// <summary>
        /// From INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A dictionary for holding properties and their values.
        /// </summary>
        private Dictionary<string, object> propertyCollection = null;

        public ViewModelBase()
        {
            propertyCollection = new Dictionary<string, object>();
        }

        protected IoCContainer GetViewModelIoCContainer()
        {
            return IoC.GetContainerForViewModel(this.GetType());
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The property to raise the event for.</param>
        protected internal void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            var dispatcher = IoC.Current.Resolve<IUIDispatcher>();
            if (dispatcher.HasThreadAccess)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                dispatcher.RunAsync(() => RaisePropertyChanged(propertyName)).Wait();
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyKey">The property key that corresponds to the property to raise the event for.</param>
        protected internal void RaisePropertyChanged(ViewModelPropertyKey propertyKey)
        {
            if (propertyKey == null) throw new ArgumentNullException("propertyKey");

            RaisePropertyChanged(propertyKey.PropertyName);
        }

        /// <summary>
        /// Returns a key that corresponds to a property/
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns></returns>
        protected ViewModelPropertyKey GetProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            return new ViewModelPropertyKey(this, propertyName);
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type to return the value as.</typeparam>
        /// <param name="propertyKey">The key that corresponds to a property.</param>
        /// <returns></returns>
        protected T GetPropertyValue<T>(ViewModelPropertyKey propertyKey)
        {
            if (propertyKey == null) throw new ArgumentNullException("propertyKey");

            return GetPropertyValue<T>(propertyKey.PropertyName);
        }
        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type to return the value as.</typeparam>
        /// <param name="propertyName">The property to return the value of/</param>
        /// <returns></returns>
        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            return propertyCollection.ContainsKey(propertyName) ? (T)propertyCollection[propertyName] : default(T);
        }
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type parameter of the value to be set.</typeparam>
        /// <param name="propertyKey">The key that corresponds to the property to be set.</param>
        /// <param name="value">The value to be set.</param>
        protected void SetPropertyValue<T>(ViewModelPropertyKey propertyKey, T value)
        {
            if (propertyKey == null) throw new ArgumentNullException("propertyKey");

            SetPropertyValue<T>(propertyKey.PropertyName, value);
        }
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type parameter of the value to be set.</typeparam>
        /// <param name="propertyName">The name of the property to be set.</param>
        /// <param name="value">The value to be set.</param>
        protected void SetPropertyValue<T>([CallerMemberName] string propertyName = "", T value = default(T))
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            SetPropertyValueSuppressPropertyChanged<T>(propertyName, value);

            RaisePropertyChanged(propertyName); //Raises the property changed event for the property.
        }
        protected void SetPropertyValueSuppressPropertyChanged<T>([CallerMemberName] string propertyName = "", T value = default(T))
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            //Sets the property if it exists or adds it if it doesn't.
            if (propertyCollection.ContainsKey(propertyName))
            {
                propertyCollection[propertyName] = value; //Sets the property.
            }
            else
                propertyCollection.Add(propertyName, value); //Adds the property.
        }

        public Task<T> WaitForPropertyChangeAsync<T>(string propertyName)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

            PropertyChangedEventHandler handler = null;
            handler = new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs args) =>
            {
                if (args.PropertyName == propertyName)
                {
                    this.PropertyChanged -= handler;

                    taskSource.SetResult(GetPropertyValue<T>(args.PropertyName));
                }
            });

            this.PropertyChanged += handler;

            return taskSource.Task;
        }
    }
}
