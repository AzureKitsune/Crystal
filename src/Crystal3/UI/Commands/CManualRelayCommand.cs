using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Crystal3.UI.Commands
{
    public class CManualRelayCommand: CRelayCommand, ICommand
    {
        public new event EventHandler CanExecuteChanged;

        public CManualRelayCommand(Action<object> executePredicate): base(executePredicate)
        {

        }

        public CManualRelayCommand(Action<object> executePredicate, Func<object, bool> canExecutePredicate) : base(executePredicate, canExecutePredicate)
        {

        }

        public void RaiseCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
