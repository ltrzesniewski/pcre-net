using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if TEST_BUILD
[assembly: InternalsVisibleTo("PCRE.NET.Tests")]
#else
[assembly: InternalsVisibleTo("PCRE.NET.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f155bb9aacf1d75e12075cf2dc0119341b318c8c322be0320af5d7a6ed6e24a33be604d0fbbd617092c8efb4890a4d02373b2c581ef82c5f811fb99013336e82fa3dbe4dcaea611c9d408e6143eb96ba00eaf8623b54dfea69160157df1cce3b86cd940d526c7ecf779651b896040ea391849732c06193593e5dcbbf514b24a2")]
#endif

[module: SkipLocalsInit]

[assembly: SuppressMessage("Microsoft.Design", "CA1014")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028")]
[assembly: SuppressMessage("Microsoft.Design", "CA1034")]
