using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal3.Messaging
{
    public class Message
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        internal Message(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
