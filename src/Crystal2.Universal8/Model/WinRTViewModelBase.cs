using Crystal2.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Model
{
    public abstract class WinRTViewModelBase: ViewModelBase, IStateHandlingViewModel
    {
        public virtual void OnRestore(IDictionary<string, object> data)
        {
            return;
        }

        public virtual void OnPreserve(IDictionary<string, object> data)
        {
            return;
        }
    }
}
