// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crc32.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Skia
{
    using System.Security.Cryptography;

    /// <summary>
    /// The crc 32.
    /// </summary>
    public class Crc32 : HashAlgorithm
    {
        #region Constants
        /// <summary>
        /// The default polynomial.
        /// </summary>
        public const uint DefaultPolynomial = 0xedb88320;

        /// <summary>
        /// The default seed.
        /// </summary>
        public const uint DefaultSeed = 0xffffffff;

        /// <summary>
        /// The default table.
        /// </summary>
        private static uint[] _defaultTable;
        #endregion

        #region Fields
        /// <summary>
        /// The seed.
        /// </summary>
        private readonly uint _seed;

        /// <summary>
        /// The table.
        /// </summary>
        private readonly uint[] _table;

        /// <summary>
        /// The hash.
        /// </summary>
        private uint _hash;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        public Crc32()
        {
            _table = InitializeTable(DefaultPolynomial);
            _seed = DefaultSeed;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crc32"/> class.
        /// </summary>
        /// <param name="polynomial">The polynomial.</param>
        /// <param name="seed">The seed.</param>
        public Crc32(uint polynomial, uint seed)
        {
            _table = InitializeTable(polynomial);
            _seed = seed;

            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the hash size.
        /// </summary>
        public override int HashSize
        {
            get { return 32; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// The compute.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint Compute(byte[] buffer)
        {
            return ~ToNewRangeHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The compute.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint Compute(uint seed, byte[] buffer)
        {
            return ~ToNewRangeHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The compute.
        /// </summary>
        /// <param name="polynomial">The polynomial.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public static uint Compute(uint polynomial, uint seed, byte[] buffer)
        {
            return ~ToNewRangeHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public override void Initialize()
        {
            _hash = _seed;
        }

        /// <summary>
        /// The hash core.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        protected override void HashCore(byte[] buffer, int start, int length)
        {
            _hash = ToNewRangeHash(_table, _hash, buffer, start, length);
        }

        /// <summary>
        /// The hash final.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~_hash);

#if !NETFX_CORE
            HashValue = hashBuffer;
#endif

            return hashBuffer;
        }

        /// <summary>
        /// The initialize table.
        /// </summary>
        /// <param name="polynomial">The polynomial.</param>
        /// <returns>System.UInt32[][].</returns>
        private static uint[] InitializeTable(uint polynomial)
        {
            if (polynomial == DefaultPolynomial && _defaultTable != null)
            {
                return _defaultTable;
            }

            var createTable = new uint[256];
            for (var i = 0; i < 256; i++)
            {
                var entry = (uint)i;
                for (var j = 0; j < 8; j++)
                {
                    if ((entry & 1) == 1)
                    {
                        entry = (entry >> 1) ^ polynomial;
                    }
                    else
                    {
                        entry = entry >> 1;
                    }
                }

                createTable[i] = entry;
            }

            if (polynomial == DefaultPolynomial)
            {
                _defaultTable = createTable;
            }

            return createTable;
        }

        /// <summary>
        /// The calculate hash.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="size">The size.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        private static uint ToNewRangeHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            var crc = seed;
            for (var i = start; i < size; i++)
            {
                unchecked
                {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                }
            }

            return crc;
        }

        /// <summary>
        /// The unsigned integer to big endian bytes.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.Byte[][].</returns>
        private static byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new[]
            {
                (byte) ((x >> 24) & 0xff), (byte) ((x >> 16) & 0xff),
                (byte) ((x >> 8) & 0xff), (byte) (x & 0xff)
            };
        }
        #endregion
    }
}