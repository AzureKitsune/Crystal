using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crystal2.Actions
{
    public class CrystalRelayCommand: ICommand
    {
        //not exactly the best variable names considering they are similar to the parameters in the constructor.
        Func<object, bool> canExecuteFunction = null;
        Action<object> executeFunction = null;

        public CrystalRelayCommand(Action<object> _executeFunction) : this((x) => true, _executeFunction) { }
        public CrystalRelayCommand(Func<object, bool> _canExecuteFunction, Action<object> _executeFunction)
        {
            if (_canExecuteFunction == null) throw new ArgumentNullException("_canExecuteFunction");
            if (_executeFunction == null) throw new ArgumentNullException("_executeFunction");

            canExecuteFunction = _canExecuteFunction;
            executeFunction = _executeFunction;
        }

        [DebuggerNonUserCode]
        public bool CanExecute(object parameter)
        {
            return canExecuteFunction(parameter);
        }

        public event EventHandler CanExecuteChanged;

        [DebuggerNonUserCode]
        public void Execute(object parameter)
        {
            executeFunction(parameter);
        }
    }
}
