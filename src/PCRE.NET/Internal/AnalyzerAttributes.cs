using System;
using System.Diagnostics;

namespace PCRE.Internal;

[Conditional("PCRENET_INTERNAL")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
internal sealed class ForwardTo8BitAttribute : Attribute;
