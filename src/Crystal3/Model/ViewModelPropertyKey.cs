using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal3.Model
{
    /// <summary>
    /// A key designed to act as a strong-typed reference to a property in a ViewModel.
    /// </summary>
    public sealed class ViewModelPropertyKey: IDisposable
    {
        private ViewModelBase viewModelBase;

        /// <summary>
        /// Gets the name of the property this key corresponds to.
        /// </summary>
        public string PropertyName { get; private set; }

        internal ViewModelPropertyKey(ViewModelBase viewModelBase, string propertyName)
        {
            this.viewModelBase = viewModelBase;
            PropertyName = propertyName;

            this.viewModelBase.PropertyChanged += viewModelBase_PropertyChanged;
        }

        void viewModelBase_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.PropertyName)
            {
                //do something if the value changed.
            }
        }

        /// <summary>
        /// Releases any internal event handlers and such.
        /// </summary>
        public void Dispose()
        {
            this.viewModelBase.PropertyChanged -= viewModelBase_PropertyChanged; 
        }
    }
}
