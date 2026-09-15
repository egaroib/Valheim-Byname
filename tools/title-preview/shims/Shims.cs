using System;
using System.Collections.Generic;
using System.Linq;
using Byname.Titles;

namespace Byname
{
    internal static class BynamePlugin
    {
        internal static void LogError(string m) => Console.Error.WriteLine("ERROR " + m);
        internal static void LogVerbose(string m) { }
    }
}

namespace Byname.Config
{
    internal sealed class Entry<T> { public T Value; public Entry(T v) { Value = v; } }

    /// Stub carrying the SAME default values the real BynameConfig binds.
    internal static class BynameConfig
    {
        internal static Entry<bool> VerboseLogging = new Entry<bool>(false);
        internal static Entry<int> MaxTitleLength = new Entry<int>(32);
        internal static HashSet<TitleCategory> Disabled = new HashSet<TitleCategory>();
        internal static HashSet<string> Blocked = new HashSet<string>();
        internal static bool CategoryEnabled(TitleCategory c) => !Disabled.Contains(c);
        internal static bool IsBlocked(string id) => Blocked.Contains(id);
    }
}
