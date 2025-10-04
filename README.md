# WinVer - Accurate Windows Version Detection for .NET

A modern .NET 9.0 class library that provides accurate Windows version detection, born from the need to update an outdated StackOverflow answer about detecting Windows 7.

## Background

This project originated from [my StackOverflow answer](https://stackoverflow.com/a/2102511/44375) to "How to determine if O/S is Windows 7?" that became outdated over time. While the original answer served its purpose, commenters noted it no longer worked reliably with modern Windows versions due to compatibility shims and deprecated APIs. This library provides a modern, accurate solution using native Windows APIs.

## Why This Library Exists

Traditional .NET methods for detecting Windows versions have several limitations:

- `Environment.OSVersion` returns compatibility-shimmed values
- `RuntimeInformation.OSDescription` provides strings that are hard to parse
- Many online examples use deprecated or unreliable methods
- Distinguishing between client and server editions is often overlooked

WinVer solves these problems by calling the native `RtlGetVersion` function directly, bypassing Windows compatibility layers to provide accurate version information.

## How It Works

The library uses P/Invoke to call `RtlGetVersion` from `ntdll.dll`, which is the same function Windows uses internally to report version information. This approach:

1. **Bypasses compatibility shims** - Gets the real OS version, not what Windows thinks your app can handle
2. **Provides detailed information** - Major/minor versions, build numbers, and product types
3. **Distinguishes editions** - Can tell the difference between Windows 10/11, Server 2016/2019/2022, etc.
4. **Caches results** - Avoids repeated native calls for better performance

## Usage

```csharp
using WinVer;

// Get friendly name (e.g., "Windows 11", "Windows Server 2022")
Console.WriteLine($"OS: {WindowsVersion.FriendlyName}");

// Get detailed version info
Console.WriteLine($"Version: {WindowsVersion.VersionString}"); // e.g., "10.0.22631"
Console.WriteLine($"Build: {WindowsVersion.BuildNumber}");

// Check edition type
if (WindowsVersion.IsServer)
    Console.WriteLine("Running on Windows Server");
else if (WindowsVersion.IsWorkstation)
    Console.WriteLine("Running on Windows client/workstation");

// Check for specific versions
if (WindowsVersion.MajorVersion == 10 && WindowsVersion.BuildNumber >= 22000)
    Console.WriteLine("This is Windows 11 or later");
```

## Building and Testing

```bash
# Build the entire solution
dotnet build

# Run the test application to see version detection in action
dotnet run --project WinVer.Test

# Build for release
dotnet build -c Release
```

## Technical Notes

### Why RtlGetVersion?

- **GetVersionEx** - Deprecated since Windows 8.1, returns compatibility-shimmed values
- **VerifyVersionInfo** - Complex to use and still subject to compatibility behavior
- **RtlGetVersion** - Undocumented but stable, used internally by Windows, returns real version info

### Platform Support

This library is Windows-specific and requires .NET 9.0 or later. The main class is decorated with `[SupportedOSPlatform("windows")]` to indicate platform requirements.

### Version Mapping

The library includes comprehensive mapping for:
- **Windows 11** (Build 22000+)
- **Windows 10** (Build 10240+)
- **Windows Server 2022** (Build 20348+)
- **Windows Server 2019** (Build 17763+)
- **Windows Server 2016** (Build 14393+)
- Legacy versions back to Windows 2000

### Product Type Detection

Uses the `wProductType` field from `OSVERSIONINFOEX`:
- `VER_NT_WORKSTATION` (1) - Client/workstation editions
- `VER_NT_DOMAIN_CONTROLLER` (2) - Domain controller
- `VER_NT_SERVER` (3) - Server editions

## Thread Safety

The library uses a simple caching mechanism with a nullable static field. While not explicitly thread-safe, the worst-case scenario is multiple threads calling the native function simultaneously, which is harmless.

## License

This project is provided as-is for educational and practical use. Feel free to use, modify, and distribute according to your needs.

## Contributing

Found a Windows version that isn't properly detected? Please open an issue with the version details and expected friendly name.