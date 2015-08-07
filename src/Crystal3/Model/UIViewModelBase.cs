using Crystal3.Navigation;
using Crystal3.UI.StatusManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3.Model
{
    public abstract class UIViewModelBase: ViewModelBase
    {
        public UIViewModelBase()
        {
           Status = WindowManager.GetStatusManagerForCurrentWindow();
        }

        public StatusManager Status { get; private set; }
    }
}
