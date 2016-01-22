using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crystal3.UI.Commands
{
    /// <summary>
    /// Crystal's version of a RelayCommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected Func<object, bool> canExecuteFunction = null;
        protected Action<object> executeFunction = null;

        public RelayCommand(Action<object> executePredicate)
        {
            executeFunction = executePredicate;
        }
        public RelayCommand(Action<object> executePredicate, Func<object, bool> canExecutePredicate) : this(executePredicate)
        {
            canExecuteFunction = canExecutePredicate;
        }

        public virtual bool CanExecute(object parameter)
        {
            return canExecuteFunction == null ? true : canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                executeFunction(parameter);
        }

        public void RaiseCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
