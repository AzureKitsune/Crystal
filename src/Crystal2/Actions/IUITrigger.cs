using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Actions
{
    public interface IUITrigger
    {
        void OnAttach(object control);
        void OnDetach(object control);
    }
}
