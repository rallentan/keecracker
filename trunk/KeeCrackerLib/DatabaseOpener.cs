using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using NLib;

namespace KeeCracker
{
    class DatabaseOpener
    {
        //--- Constants ---

        // KeePass 1.x signature
        private const uint FileSignatureOld1 = 0x9AA2D903;
        private const uint FileSignatureOld2 = 0xB54BFB65;
        /// <summary>
        /// File identifier, first 32-bit value.
        /// </summary>
        private const uint FileSignature1 = 0x9AA2D903;
        /// <summary>
        /// File identifier, second 32-bit value.
        /// </summary>
        private const uint FileSignature2 = 0xB54BFB67;
        // KeePass 2.x pre-release (alpha and beta) signature
        private const uint FileSignaturePreRelease1 = 0x9AA2D903;
        private const uint FileSignaturePreRelease2 = 0xB54BFB66;
        private const uint FileVersionCriticalMask = 0xFFFF0000;
        /// <summary>
        /// File version of files saved by the current <c>Kdb4File</c> class.
        /// KeePass 2.07 has version 1.01, 2.08 has 1.02, 2.09 has 2.00,
        /// 2.10 has 2.02, 2.11 has 2.04, 2.15 has 3.00.
        /// The first 2 bytes are critical (i.e. loading will fail, if the
        /// file version is too high), the last 2 bytes are informational.
        /// </summary>
        private const uint FileVersion32 = 0x00030000;
        
        //--- Fields ---
        Stream _encryptedDatabase;
        long _dataStartOffset;
        ulong _transformRounds;
        byte[] _masterSeed;
        byte[] _transformSeed;
        byte[] _initializationVectors;
        byte[] _expectedStartBytes;
        byte[] _actualStartBytes = new byte[32];
        SHA256Managed _sha256 = new SHA256Managed();
        byte[] _buffer = new byte[64];
        UTF8Encoding _utf8Encoding = new UTF8Encoding();

        //--- Constructors ---

        public DatabaseOpener(Stream encryptedDatabase)
        {
            if (encryptedDatabase == null)
                throw new ArgumentNullException("encryptedDatabase");

            _encryptedDatabase = encryptedDatabase;

            ReadDbHeader(_encryptedDatabase);
            _dataStartOffset = _encryptedDatabase.Position;
        }

        //--- Public Methods ---

        public bool TryKey(string password)
        {
            byte[] passwordBytes = _utf8Encoding.GetBytes(password);
            byte[] userKey = _sha256.ComputeHash(passwordBytes);
            bool result = TryKey(userKey);
            return result;
        }

        public bool TryKey(byte[] userKey)
        {
            var rawCompositeKey = GenerateRawCompositeKey(userKey);  // returns byte[32]
            var transformedRawCompositeKey = TransformKey(rawCompositeKey, _transformSeed, _transformRounds);  // returns byte[32]
            
            var aesKey = GenerateMasterKey(_masterSeed, transformedRawCompositeKey);  // Output is byte[64]

            Stream decryptedDatabase = PerformDecrypt(_encryptedDatabase, aesKey, _initializationVectors);
            _encryptedDatabase.Seek(_dataStartOffset, SeekOrigin.Begin);

            int bytesRead = decryptedDatabase.Read(_actualStartBytes, 0, 32);
            if (bytesRead != 32)
                throw new Exception();

            return _actualStartBytes.CompareTo(_expectedStartBytes);
        }

        byte[] GenerateRawCompositeKey(byte[] userKey)
        {
            //MemoryStream ms = new MemoryStream();
            //int userKeysLength = userKeys.Length;

            //for (int i = 0; i < userKeysLength; i++)
            //{
            //    ms.Write(userKeys[i], 0, userKey.Length);
            //}

            //return _sha256.ComputeHash(ms.ToArray());

            return _sha256.ComputeHash(userKey);
        }
        
        static bool TransformKey256(byte[] pBuf256, byte[] pKey256, ulong uRounds)
        {
            // Prepare arrays
            IntPtr hBuf = Marshal.AllocHGlobal(pBuf256.Length);
            Marshal.Copy(pBuf256, 0, hBuf, pBuf256.Length);

            IntPtr hKey = Marshal.AllocHGlobal(pKey256.Length);
            Marshal.Copy(pKey256, 0, hKey, pKey256.Length);

            var kvp = new KeyValuePair<IntPtr, IntPtr>(hBuf, hKey);

            // Call native method
            bool bResult = false;

            try
            {
                if (Marshal.SizeOf(typeof(IntPtr)) == 8)
                    return TransformKey64(kvp.Key, kvp.Value, uRounds);
                else
                    return TransformKey32(kvp.Key, kvp.Value, uRounds);
            }
            catch (Exception)
            {
                bResult = false;
            }

            if (bResult)
            {
                // Get buffers
                if (kvp.Key != IntPtr.Zero)
                    Marshal.Copy(kvp.Key, pBuf256, 0, pBuf256.Length);

                if (kvp.Value != IntPtr.Zero)
                    Marshal.Copy(kvp.Value, pKey256, 0, pKey256.Length);
            }

            // Free arrays
            if (kvp.Key != IntPtr.Zero)
                Marshal.FreeHGlobal(kvp.Key);

            if (kvp.Value != IntPtr.Zero)
                Marshal.FreeHGlobal(kvp.Value);

            return bResult;
        }

        byte[] TransformKey(byte[] key, byte[] transformSeed, ulong numberOfRounds)
        {
            // Try to use the native library first
            if (!TransformKey256(key, transformSeed, numberOfRounds))
                throw new Exception();

            return _sha256.ComputeHash(key);
        }

        byte[] GenerateMasterKey(byte[] masterSeed, byte[] transformedRawCompositeKey)
        {
            masterSeed.CopyTo(_buffer, 0);
            transformedRawCompositeKey.CopyTo(_buffer, 32);
            return _sha256.ComputeHash(_buffer);
        }

        Stream PerformDecrypt(Stream s, byte[] pbKey, byte[] pbIV)
        {
            RijndaelManaged r = new RijndaelManaged();

            r.IV = pbIV;

            r.KeySize = 256;
            r.Key = pbKey;

            r.Mode = CipherMode.CBC;
            r.Padding = PaddingMode.PKCS7;

            ICryptoTransform iTransform = r.CreateDecryptor();

            return new CryptoStream(s, iTransform, CryptoStreamMode.Read);
        }

        void ReadDbHeader(Stream encryptedDatabase)
        {
            BinaryReader source = new BinaryReader(encryptedDatabase);
            {
                byte[] pbSig1 = source.ReadBytes(4);
                uint uSig1 = BytesToUInt32(pbSig1);
                byte[] pbSig2 = source.ReadBytes(4);
                uint uSig2 = BytesToUInt32(pbSig2);

                if ((uSig1 == FileSignatureOld1) && (uSig2 == FileSignatureOld2))
                    throw new Exception("Old format");

                if ((uSig1 == FileSignature1) && (uSig2 == FileSignature2))
                {
                }
                else if ((uSig1 == FileSignaturePreRelease1) && (uSig2 == FileSignaturePreRelease2))
                {
                }
                else
                    throw new Exception("Unknown format: File signature invalid");

                byte[] pb = source.ReadBytes(4);
                uint uVersion = BytesToUInt32(pb);
                if ((uVersion & FileVersionCriticalMask) > (FileVersion32 & FileVersionCriticalMask))
                    throw new Exception("Unknown format: File version unsupported");

                
                bool endReached = false;

                while (!endReached)
                {
                    byte btFieldID = source.ReadByte();
                    ushort uSize = BytesToUInt16(source.ReadBytes(2));

                    byte[] pbData = null;
                    if (uSize > 0)
                    {
                        pbData = source.ReadBytes(uSize);
                    }

                    Kdb4HeaderFieldID kdbID = (Kdb4HeaderFieldID)btFieldID;
                    switch (kdbID)
                    {
                        case Kdb4HeaderFieldID.EndOfHeader:
                            endReached = true;  // end of header
                            break;

                        case Kdb4HeaderFieldID.MasterSeed:
                            _masterSeed = pbData;
                            break;

                        case Kdb4HeaderFieldID.TransformSeed:
                            _transformSeed = pbData;
                            break;
                        
                        case Kdb4HeaderFieldID.TransformRounds:
                            _transformRounds = BytesToUInt64(pbData);
                            break;

                        case Kdb4HeaderFieldID.EncryptionIV:
                            _initializationVectors = pbData;
                            break;

                        case Kdb4HeaderFieldID.StreamStartBytes:
                            _expectedStartBytes = pbData;
                            break;
                    }
                }
            }

            if (_transformRounds == 0)
                throw new Exception();
            if (_initializationVectors == null)
                throw new Exception();
            if (_expectedStartBytes == null)
                throw new Exception();
        }

        static ushort BytesToUInt16(byte[] pb)
        {
            return (ushort)((ushort)pb[0] | ((ushort)pb[1] << 8));
        }

        static uint BytesToUInt32(byte[] pb)
        {
            return
                (uint)pb[0] |
                ((uint)pb[1] << 8) |
                ((uint)pb[2] << 16) |
                ((uint)pb[3] << 24);
        }

        static ulong BytesToUInt64(byte[] pb)
        {
            return
                (ulong)pb[0] |
                ((ulong)pb[1] << 8) |
                ((ulong)pb[2] << 16) |
                ((ulong)pb[3] << 24) |
                ((ulong)pb[4] << 32) |
                ((ulong)pb[5] << 40) |
                ((ulong)pb[6] << 48) |
                ((ulong)pb[7] << 56);
        }

        [DllImport("KeePassLibC32.dll", EntryPoint = "TransformKey256")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TransformKey32(IntPtr pBuf256,
            IntPtr pKey256, UInt64 uRounds);

        [DllImport("KeePassLibC64.dll", EntryPoint = "TransformKey256")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TransformKey64(IntPtr pBuf256,
            IntPtr pKey256, UInt64 uRounds);

        //--- Nested Types ---

        enum Kdb4HeaderFieldID : byte
        {
            EndOfHeader = 0,
            Comment = 1,
            CipherID = 2,
            CompressionFlags = 3,
            MasterSeed = 4,
            TransformSeed = 5,
            TransformRounds = 6,
            EncryptionIV = 7,
            ProtectedStreamKey = 8,
            StreamStartBytes = 9,
            InnerRandomStreamID = 10
        }
    }
}
