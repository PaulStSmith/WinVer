# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WinVer is a .NET 9.0 class library that provides Windows version detection capabilities through native Windows APIs. The library can distinguish between client/workstation and server editions, retrieve detailed version information, and provide friendly names for Windows versions.

## Build Commands

- Build the entire solution: `dotnet build`
- Build in Release mode: `dotnet build -c Release`
- Clean build artifacts: `dotnet clean`
- Run the test application: `dotnet run --project WinVer.Test`

## Project Structure

- `WinVer/` - Main class library project
  - `WindowsVersion.cs` - Core static class providing Windows version detection
  - `WinVer.csproj` - Targets .NET 9.0 with nullable reference types enabled
- `WinVer.Test/` - Console application for testing the library
  - `Program.cs` - Demonstrates library usage and includes debug output
  - `WinVer.Test.csproj` - Targets .NET 9.0-windows with executable output
- `WinVer.sln` - Visual Studio solution file containing both projects

## Architecture

The library uses P/Invoke to call the native `RtlGetVersion` function from `ntdll.dll`, which provides accurate version information bypassing Windows compatibility shims. Key architectural elements:

- `OSVERSIONINFOEX` struct for interop with Windows APIs
- Caching mechanism to avoid repeated native calls
- Static properties for common version queries (MajorVersion, MinorVersion, BuildNumber)
- Product type detection (Workstation, Server, Domain Controller)
- Friendly name mapping based on version numbers and build numbers
- Platform-specific code marked with `[SupportedOSPlatform("windows")]`

The test application uses reflection to access private methods for debugging purposes, demonstrating the library's internal structure.
