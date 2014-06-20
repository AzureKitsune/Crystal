using Crystal2.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Model
{
    /// <summary>
    /// A base class for ViewModels to inherit which implements INotifyPropertyChanged.
    /// </summary>
    public abstract class ViewModelBase: INotifyPropertyChanged, INavigatingViewModel
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

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The property to raise the event for.</param>
        protected internal void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
        protected T GetPropertyValue<T>(string propertyName)
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
        protected void SetPropertyValue<T>(string propertyName, T value)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            //Sets the property if it exists or adds it if it doesn't.
            if (propertyCollection.ContainsKey(propertyName))
            {
                propertyCollection[propertyName] = value; //Sets the property.
            }
            else
                propertyCollection.Add(propertyName, value); //Adds the property.

            RaisePropertyChanged(propertyName); //Raises the property changed event for the property.
        }

        public virtual void OnNavigatingTo() { }

        public virtual void OnNavigatedTo(object parameter, CrystalNavigationEventArgs args) { }

        public virtual bool OnNavigatingFrom() { return false; }

        public virtual void OnNavigatedFrom() { }

        public virtual void OnRefresh() { }
    }
}
