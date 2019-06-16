using System;

namespace Crystal3.Navigation
{
    [Flags]
    public enum NavigationViewSupportedPlatform
    {
        All = Desktop | Mobile | Xbox | Holographic | Team | IoT,
        /// <summary>
        /// Desktop (including with Mixed Reality headsets)
        /// </summary>
        Desktop = 1,
        /// <summary>
        /// Windows 10 Mobile (and potentially W10 on ARM)
        /// </summary>
        Mobile = 2,
        /// <summary>
        /// Xbox One and Xbox Scarlet
        /// </summary>
        Xbox = 4,
        /// <summary>
        /// HoloLens
        /// </summary>
        Holographic = 8,
        /// <summary>
        /// Surface Hub
        /// </summary>
        Team = 16,
        /// <summary>
        /// IoT
        /// </summary>
        IoT = 32,
    }
}