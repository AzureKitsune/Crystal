using System;

namespace Crystal3
{
    public class CrystalApplicationShouldRestoreEventArgs: EventArgs
    {
        internal CrystalApplicationShouldRestoreEventArgs() { }

        public DateTimeOffset SuspensionFileDate { get; internal set; }
    }
}