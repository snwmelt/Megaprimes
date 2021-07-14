using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaprimesLib
{
    /// <summary>
    /// Provides a mechanism for generating sequences of Megaprime numbers.
    /// </summary>
    /// <remarks>
    /// A megaprime is defined as a prime number that is constituted of 
    /// digits that are themselves prime numbers.
    /// </remarks>
    public sealed class MegaprimeGenerator
    {
        private static readonly  UInt32             _MinBlockSize;
        private static readonly  UInt32             _MaxBlockSize;
        private static readonly  ArrayPool<Boolean> _BooleanArrayPool      = ArrayPool<Boolean>.Shared;
        private static readonly  UInt32[]           _Empty                 = Array.Empty<UInt32>( );
        private static readonly  UInt32[]           _KnownMegaprimes = new []
        {
            2U,
            3U,
            5U,
            7U
        };

        
        private static readonly HashSet<UInt32> _KnownMegaprimesSet = new (_KnownMegaprimes);

        static MegaprimeGenerator ( )
        {
            // Note: Initial Caches size values taken from intel SandyBridge reference.
            UInt32 SmallCacheSize = 32;
            UInt32 LargeCacheSize = 2000;

            if (OperatingSystem.IsWindows( ))
            {
                var l1 = CPUInfo.GetCacheSizes(CacheLevel.Level1);
                var l2 = CPUInfo.GetCacheSizes(CacheLevel.Level2);
                var l3 = CPUInfo.GetCacheSizes(CacheLevel.Level3);

                if (l1.Count > 0)
                    SmallCacheSize = l1[0];

                if (l2.Count > 0)
                    LargeCacheSize = l2[0];

                if (l3.Count > 0)
                    LargeCacheSize = l3[0];
            }

            // Note: Aligning block sizes with available cache capacity to improve performance.
            _MinBlockSize = (SmallCacheSize * 1000) / sizeof(UInt32);
            _MaxBlockSize = (LargeCacheSize * 1000) / sizeof(UInt32);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{UInt32}"/> containing 
        /// all megaprimes in the sequence 0 to <paramref name="MaxValue"/> 
        /// (inclusive).
        /// </summary>
        /// <param name="MaxValue">
        /// The maximum possible megaprime value returned by this method.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{UInt32}"/> containing megaprimes.
        /// </returns>
        public static IEnumerable<UInt32> UpTo (UInt32 MaxValue)
        {
            if (MaxValue < 2)
                return _Empty;

            if (MaxValue < 23) // Note: Using faster lookup for known values before next megaprime
                return KnownMegaprimesUpTo(MaxValue);

            return UnknownMegaprimesUpTo(MaxValue);
        }

        /// <summary>
        /// Quick loopkup of megaprime values up to <paramref name="MaxValue"/>
        /// using the <see cref="_KnownMegaprimes"/> collection.
        /// </summary>
        /// <inheritdoc cref="UpTo(UInt32)"/>
        /// <param name="MaxValue"/>
        /// <returns/>
        private static IEnumerable<UInt32> KnownMegaprimesUpTo (UInt32 MaxValue)
        {
            var Value = 0U;

            for (UInt32 i = 0; MaxValue > Value && i < _KnownMegaprimes.Length; i++)
            {
                Value = _KnownMegaprimes[i];

                yield return Value;
            }
        }

        private static UInt32 GetBlockSizeFor (UInt32 MaxValue)
            => Math.Min(Math.Max(MaxValue / (UInt32)(Environment.ProcessorCount / 2), _MinBlockSize), _MaxBlockSize);

        /// <summary>
        /// Loopkup of megaprime values up to <paramref name="MaxValue"/> 
        /// using the Sieve of Eratosthenes algorithm.
        /// </summary>
        /// <see href="https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes"/>
        /// <inheritdoc cref="UpTo(UInt32)"/>
        /// <param name="MaxValue"/>
        /// <returns/>
        private static IEnumerable<UInt32> UnknownMegaprimesUpTo (UInt32 MaxValue)
        {
            var MaxValueSqrt       = (UInt32)Math.Sqrt(MaxValue);
            var NonPrime           = new Boolean[MaxValueSqrt + 2];
            var MaxValueSqrtPrimes = new List<UInt32>( );
            var BlockSize          = GetBlockSizeFor(MaxValue);
            var ProcessedBlockSize = 0UL;


            for (UInt32 i = 2; i <= MaxValueSqrt; i++)
            {
                if (!NonPrime[i])
                {
                    MaxValueSqrtPrimes.Add(i);

                    for (UInt32 j = i * i; j <= MaxValueSqrt; j += i)
                        NonPrime[j] = true;
                }
            }

            // Note : We explore the chunks in UInt64 space to avoid potential overflows 
            //        when the specified MaxValue is at or near UInt32.MaxValue.
            for (UInt64 i = 0; ProcessedBlockSize <= MaxValue; ProcessedBlockSize = (i * BlockSize))
            {
                foreach (var Prime in MegaprimesInBlock(i, MaxValueSqrtPrimes, (UInt32)ProcessedBlockSize, Math.Min(BlockSize, (MaxValue - ProcessedBlockSize))))
                    yield return Prime;

                i++;
            }

            if (IsMegaprime(MaxValue))
                yield return MaxValue;
        }

        private static IEnumerable<UInt32> MegaprimesInBlock (UInt64 BlockIndex, ICollection<UInt32> MaxValueSqrtPrimes, UInt32 BlockStart, UInt64 BlockSize)
        {
            var NonPrime = _BooleanArrayPool.Rent((Int32)BlockSize);

            try
            {
                Parallel.ForEach(MaxValueSqrtPrimes, Offset =>
                {
                    var BlockStartOffset = (BlockStart + Offset -1) / Offset;

                    for (UInt64 i = (Math.Max(BlockStartOffset, Offset) * Offset) - BlockStart; i < BlockSize; i += Offset)
                        NonPrime[i] = true;
                });

                if (BlockIndex == 0)
                    NonPrime[0] = NonPrime[1] = true;

                for (UInt32 i = 0; i < BlockSize; i++)
                {
                    if (!NonPrime[i] && IsMegaprime(BlockStart))
                        yield return BlockStart;

                    BlockStart++;
                }
            }
            finally
            {
                _BooleanArrayPool.Return(NonPrime, true);
            }
        }

        private static Boolean IsMegaprime (UInt32 Value)
        {
            while (_KnownMegaprimesSet.Contains(Value % 10))
                Value /= 10;

            return Value == 0;
        }
    }
}
