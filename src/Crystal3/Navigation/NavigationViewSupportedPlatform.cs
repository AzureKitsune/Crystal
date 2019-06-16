using System;

namespace Crystal3.Navigation
{
    [Flags]
    public enum NavigationViewSupportedPlatform
    {
        All = Desktop | Mobile | Xbox | Holographic | Team | IoT,
        Desktop = 1,
        Mobile = 2,
        Xbox = 4,
        Holographic = 8,
        /// <summary>
        /// Surface Hub
        /// </summary>
        Team = 16,
        IoT = 32,
        MixedRealityHolographic = Desktop | Holographic
    }
}