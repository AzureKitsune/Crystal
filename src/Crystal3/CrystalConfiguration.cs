using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crystal3
{
    public sealed class CrystalConfiguration
    {
        internal CrystalConfiguration()
        {
            ViewModelResumeMethod = ViewModelResumeMethod.ResumingAsync;
        }

        public bool HandleSystemBackNavigation { get; set; }
        public bool HandleBackButtonForTopLevelNavigation { get; set; }
        public ViewModelResumeMethod ViewModelResumeMethod { get; internal set; }
    }
}
