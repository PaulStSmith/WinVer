using WinVer;
using System.Reflection;

Console.WriteLine("Windows Version Information");
Console.WriteLine("============================");
Console.WriteLine();
Console.WriteLine($"Friendly Name:    {WindowsVersion.FriendlyName}");
Console.WriteLine($"Version String:   {WindowsVersion.VersionString}");
Console.WriteLine($"Major Version:    {WindowsVersion.MajorVersion}");
Console.WriteLine($"Minor Version:    {WindowsVersion.MinorVersion}");
Console.WriteLine($"Build Number:     {WindowsVersion.BuildNumber}");
Console.WriteLine();
Console.WriteLine($"Is Workstation:   {WindowsVersion.IsWorkstation}");
Console.WriteLine($"Is Server:        {WindowsVersion.IsServer}");
Console.WriteLine($"Is Domain Ctrl:   {WindowsVersion.IsDomainController}");

// Debug: Get the actual wProductType value
var type = typeof(WindowsVersion);
var method = type.GetMethod("GetVersionInfo", BindingFlags.NonPublic | BindingFlags.Static);
if (method != null)
{
    var versionInfo = method.Invoke(null, null);
    if (versionInfo != null)
    {
        var productTypeField = versionInfo.GetType().GetField("wProductType");
        if (productTypeField != null)
        {
            var productType = productTypeField.GetValue(versionInfo);
            Console.WriteLine();
            Console.WriteLine($"DEBUG: wProductType = {productType} (0x{productType:X})");
        }
    }
}
