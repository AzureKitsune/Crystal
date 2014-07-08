using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Crystal2.State
{
    [DataContract]
    public class StateObject
    {
        [DataMember]
        public string NavigationState { get; set; }
        [DataMember]
        public Collection<object[]> StateObjects { get; set; }
    }
}
