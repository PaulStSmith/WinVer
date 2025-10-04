using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WinVer
{
    /// <summary>
    /// Provides information about the Windows version, including the ability to distinguish between client and server editions.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class WindowsVersion
    {
        /// <summary>
        /// Calls the native RtlGetVersion function from ntdll.dll to retrieve Windows version information.
        /// </summary>
        /// <param name="versionInfo">A reference to an <see cref="OSVERSIONINFOEX"/> structure that receives the version information.</param>
        /// <returns>
        /// Returns 0 if successful; otherwise, returns a nonzero error code.
        /// </returns>
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion(ref OSVERSIONINFOEX versionInfo);

        /// <summary>
        /// Represents extended information about the Windows operating system version.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OSVERSIONINFOEX
        {
            /// <summary>
            /// The size of this data structure, in bytes.
            /// </summary>
            public int dwOSVersionInfoSize;
            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            public int dwMajorVersion;
            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            public int dwMinorVersion;
            /// <summary>
            /// The build number of the operating system.
            /// </summary>
            public int dwBuildNumber;
            /// <summary>
            /// The operating system platform.
            /// </summary>
            public int dwPlatformId;
            /// <summary>
            /// Additional information about the operating system.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            /// <summary>
            /// The major version number of the latest Service Pack installed.
            /// </summary>
            public ushort wServicePackMajor;
            /// <summary>
            /// The minor version number of the latest Service Pack installed.
            /// </summary>
            public ushort wServicePackMinor;
            /// <summary>
            /// Bit mask identifying the product suites available on the system.
            /// </summary>
            public ushort wSuiteMask;
            /// <summary>
            /// Identifies the product type.
            /// </summary>
            public byte wProductType;
            /// <summary>
            /// Reserved for future use.
            /// </summary>
            public byte wReserved;
        }

        /// <summary>
        /// Indicates a workstation product type.
        /// </summary>
        private const byte VER_NT_WORKSTATION = 0x0000001;
        /// <summary>
        /// Indicates a domain controller product type.
        /// </summary>
        private const byte VER_NT_DOMAIN_CONTROLLER = 0x0000002;
        /// <summary>
        /// Indicates a server product type.
        /// </summary>
        private const byte VER_NT_SERVER = 0x0000003;

        /// <summary>
        /// Caches the retrieved Windows version information for subsequent calls.
        /// </summary>
        private static OSVERSIONINFOEX? _cachedVersionInfo;

        /// <summary>
        /// Gets the major version number of the operating system.
        /// </summary>
        public static int MajorVersion => GetVersionInfo().dwMajorVersion;

        /// <summary>
        /// Gets the minor version number of the operating system.
        /// </summary>
        public static int MinorVersion => GetVersionInfo().dwMinorVersion;

        /// <summary>
        /// Gets the build number of the operating system.
        /// </summary>
        public static int BuildNumber => GetVersionInfo().dwBuildNumber;

        /// <summary>
        /// Gets a value indicating whether the operating system is a Windows Server edition.
        /// </summary>
        public static bool IsServer => GetVersionInfo().wProductType != VER_NT_WORKSTATION;

        /// <summary>
        /// Gets a value indicating whether the operating system is a Windows client/workstation edition.
        /// </summary>
        public static bool IsWorkstation => GetVersionInfo().wProductType == VER_NT_WORKSTATION;

        /// <summary>
        /// Gets a value indicating whether the operating system is a domain controller.
        /// </summary>
        public static bool IsDomainController => GetVersionInfo().wProductType == VER_NT_DOMAIN_CONTROLLER;

        /// <summary>
        /// Gets a friendly name for the Windows version, such as "Windows 11" or "Windows Server 2022".
        /// </summary>
        public static string FriendlyName
        {
            get
            {
                var info = GetVersionInfo();
                var isServer = info.wProductType != VER_NT_WORKSTATION;

                return (info.dwMajorVersion, info.dwMinorVersion, isServer) switch
                {
                    (10, 0, false) when info.dwBuildNumber >= 22000 => "Windows 11",
                    (10, 0, false) => "Windows 10",
                    (10, 0, true) when info.dwBuildNumber >= 20348 => "Windows Server 2022",
                    (10, 0, true) when info.dwBuildNumber >= 17763 => "Windows Server 2019",
                    (10, 0, true) => "Windows Server 2016",
                    (6, 3, false) => "Windows 8.1",
                    (6, 3, true) => "Windows Server 2012 R2",
                    (6, 2, false) => "Windows 8",
                    (6, 2, true) => "Windows Server 2012",
                    (6, 1, false) => "Windows 7",
                    (6, 1, true) => "Windows Server 2008 R2",
                    (6, 0, false) => "Windows Vista",
                    (6, 0, true) => "Windows Server 2008",
                    (5, 2, true) => "Windows Server 2003",
                    (5, 1, _) => "Windows XP",
                    (5, 0, _) => "Windows 2000",
                    _ => $"Windows {info.dwMajorVersion}.{info.dwMinorVersion}" + (isServer ? " Server" : "")
                };
            }
        }

        /// <summary>
        /// Gets the full version string of the operating system, including major, minor, and build numbers.
        /// </summary>
        public static string VersionString
        {
            get
            {
                var info = GetVersionInfo();
                return $"{info.dwMajorVersion}.{info.dwMinorVersion}.{info.dwBuildNumber}";
            }
        }

        /// <summary>
        /// Retrieves the Windows version information, using a cached value if available.
        /// </summary>
        /// <returns>
        /// An <see cref="OSVERSIONINFOEX"/> structure containing the version information.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the native call to retrieve version information fails.
        /// </exception>
        private static OSVERSIONINFOEX GetVersionInfo()
        {
            if (_cachedVersionInfo.HasValue)
            {
                return _cachedVersionInfo.Value;
            }

            var osVersionInfo = new OSVERSIONINFOEX
            {
                dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX))
            };

            if (RtlGetVersion(ref osVersionInfo) != 0)
            {
                throw new InvalidOperationException("Failed to get Windows version information.");
            }

            _cachedVersionInfo = osVersionInfo;
            return osVersionInfo;
        }
    }
}
