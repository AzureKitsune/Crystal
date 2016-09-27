using System;
using Windows.ApplicationModel.Activation;

namespace Crystal3
{
    public class CrystalApplicationBackgroundActivationEventArgs : EventArgs
    {
        public BackgroundActivatedEventArgs ActivationEventArgs { get; internal set; }
    }
}