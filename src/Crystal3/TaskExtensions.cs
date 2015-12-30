using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Crystal3
{
    public static class TaskExtensions
    {
        [MethodImpl(Met‌​hodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task)
        {
            //http://stackoverflow.com/a/22630057

            task.ConfigureAwait(false);
        }
    }
}
