using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.State
{
    internal interface IStateHandlingViewModel
    {
        void OnRestore(IDictionary<string, object> data);
        void OnPreserve(IDictionary<string, object> data);
    }
}
