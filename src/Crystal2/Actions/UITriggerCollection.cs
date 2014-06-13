using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Actions
{
    public sealed class UITriggerCollection: Collection<IUITrigger>
    {
        public UITriggerCollection():base()
        {

        }
    }
}
