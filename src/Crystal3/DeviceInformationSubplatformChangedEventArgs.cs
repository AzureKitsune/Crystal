using Crystal3.Core;
using System;

namespace Crystal3
{
    public class DeviceInformationSubplatformChangedEventArgs: EventArgs
    {
        internal DeviceInformationSubplatformChangedEventArgs(Subplatform subplatform, DeviceInformationSubplatformChangedStatus changedStatus)
        {
            this.Subplatform = subplatform;
            this.Status = changedStatus;
        }

        public Subplatform Subplatform { get; private set; }
        public DeviceInformationSubplatformChangedStatus Status { get; private set; }

        public enum DeviceInformationSubplatformChangedStatus
        {
            Activation,
            Deactivation
        }
    }
}