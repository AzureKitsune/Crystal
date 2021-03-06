﻿using Crystal3.Core;
using Crystal3.InversionOfControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    /// <summary>
    /// A basic class for models.
    /// </summary>
    [DataContract]
    public abstract class ModelBase: INotifyPropertyChanged
    {
        /// <summary>
        /// From INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A dictionary for holding properties and their values.
        /// </summary>
        private Dictionary<string, object> propertyCollection = null;

        public ModelBase()
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
        /// <param name="propertyName">The name of the property to be set.</param>
        /// <param name="value">The value to be set.</param>
        protected void SetPropertyValue<T>([CallerMemberName] string propertyName = "", T value = default(T))
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
