using System.Reflection;
using System.Runtime.InteropServices;

#pragma warning disable CS0436 // Type conflicts with imported type
[assembly: AssemblyVersion(GlobalAssemblyInformation.AssemblyVersion)]
[assembly: AssemblyCompany(GlobalAssemblyInformation.CompanyName)]
[assembly: AssemblyConfiguration(GlobalAssemblyInformation.Configuration)]
[assembly: AssemblyFileVersion(GlobalAssemblyInformation.AssemblyVersion)]
[assembly: AssemblyProduct(GlobalAssemblyInformation.ProductName)]
[assembly: AssemblyCopyright("Copyright © 2024 " + GlobalAssemblyInformation.CompanyName)]
[assembly: AssemblyTrademark(GlobalAssemblyInformation.CompanyName)]
#pragma warning restore CS0436 // Type conflicts with imported type
[assembly: ComVisible(false)]

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "<Pending>")]
static class GlobalAssemblyInformation
{
    public const string AssemblyVersion = "1.0.0.0";
    public const string CompanyName = "JetDevel";
    public const string ProductName = "JetDevel json path";
    public const string PublicKey =
        "00240000048000009400000006020000002400005253413100040000010001008919ad97b1e62aae" +
        "79c2201121c505dc2277b0cf27082d8bec6b3f82bc3d59540429e0d8fc359e985723b66e7ca7ab0d" +
        "793f90e3fb7c73d66abb13b2a655075eaed6d744006c47cb6ff105ad4255eab75c0120d76583a0a7" +
        "37dc8e89c1e60b65f534266f33071bd4b71f6b8b93a32f2662b2547f58a0271e8935f41f0107a5cd";
    public const string Configuration =
#if DEBUG
    "Debug";
#else
    "Release";
#endif
}