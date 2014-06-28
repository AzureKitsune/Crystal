using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.State
{
    internal interface IStateHandlingViewModel
    {
        void OnResume(IDictionary<string, object> data);
        void OnSuspend(IDictionary<string, object> data);
    }
}
