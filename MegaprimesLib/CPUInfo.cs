using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;

namespace MegaprimesLib
{
    internal enum CacheLevel : ushort
    {
        Level1 = 3,
        Level2 = 4,
        Level3 = 5,
    }

    internal static class CPUInfo
    {
        [SupportedOSPlatform("windows")]
        public static List<uint> GetCacheSizes (CacheLevel level)
        {
            ManagementClass mc = new ("Win32_CacheMemory");
            ManagementObjectCollection moc = mc.GetInstances();
            List<uint> cacheSizes = new (moc.Count);

            cacheSizes.AddRange(moc.Cast<ManagementObject>( )
                                   .Where(p => (ushort)(p.Properties["Level"].Value) == (ushort)level)
                                   .Select(p => (uint)(p.Properties["MaxCacheSize"].Value)));

            return cacheSizes;
        }
    }
}
