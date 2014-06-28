using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal2.State
{
    public class StateObject
    {
        public string NavigationState { get; internal set; }
        public Dictionary<string, object> StateObjects { get; internal set; }
    }
}
