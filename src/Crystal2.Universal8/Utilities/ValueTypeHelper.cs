using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crystal2.Utilities
{
    public static class ValueTypeHelper
    {
        public static bool IsConsideredValueType(object obj)
        {
            return obj is string || obj is int || obj is double || obj is float || obj is char || obj is byte || obj is uint; //tbc
        }
    }
}
