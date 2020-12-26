using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptography.Wrappers
{
    public class RsaEncryptor : IEncryptor
    {
        private readonly Lazy<RSA> _encryptionRSA;
        private readonly RSA _decryptionRSA;
        private readonly RSAEncryptionPadding _encryptionPadding;
        private readonly int _blockSize;
        private readonly int _maxDataSize;

        public RsaEncryptor(X509Certificate2 certificate, RSAEncryptionPadding encryptionPadding)
        {
            _encryptionPadding = encryptionPadding.NullValidate(nameof(encryptionPadding));

            certificate.NullValidate(nameof(certificate));
            if (!certificate.HasPrivateKey)
                throw new CryptographicException($"Certificate Thumbprint={certificate.Thumbprint} does not have private key");

            _encryptionRSA = new Lazy<RSA>(() => certificate.GetRSAPublicKey());
            _decryptionRSA = certificate.GetRSAPrivateKey();

            _maxDataSize = GetMaxDataSize(certificate, _encryptionPadding);
            _blockSize = _decryptionRSA.ExportParameters(false).Modulus.Length;
        }

        public RsaEncryptor(X509Certificate2 certificate) : this(certificate, RSAEncryptionPadding.Pkcs1)
        { }

        public byte[] Decrypt(byte[] encryptedData)
        {
            encryptedData.ArrayNullOrEmptyValidate(nameof(encryptedData));

            var decryptedDataBlocks = new List<byte[]>();
            foreach (var dataBlock in Split(encryptedData, _blockSize))
                decryptedDataBlocks.Add(_decryptionRSA.Decrypt(dataBlock, _encryptionPadding));

            return Combine(decryptedDataBlocks);
        }

        public byte[] Encrypt(byte[] data)
        {
            data.ArrayNullOrEmptyValidate(nameof(data));

            var encryptedDataBlocks = new List<byte[]>();
            foreach (var dataBlock in Split(data, _maxDataSize))
                encryptedDataBlocks.Add(_encryptionRSA.Value.Encrypt(dataBlock, _encryptionPadding));

            return Combine(encryptedDataBlocks);
        }

        //https://crypto.stackexchange.com/questions/42097/what-is-the-maximum-size-of-the-plaintext-message-for-rsa-oaep
        private int GetMaxDataSize(X509Certificate2 certificate, RSAEncryptionPadding encryptionPadding)
        {
            int hashLength;

            switch (encryptionPadding)
            {
                case var p when p == RSAEncryptionPadding.Pkcs1 || p == RSAEncryptionPadding.OaepSHA1:
                    hashLength = 160;
                    break;
                case var p when p == RSAEncryptionPadding.OaepSHA256:
                    hashLength = 256;
                    break;
                case var p when p == RSAEncryptionPadding.OaepSHA384:
                    hashLength = 384;
                    break;
                case var p when p == RSAEncryptionPadding.OaepSHA512:
                    {
                        if (certificate.PrivateKey.KeySize < 2048)
                            throw new ArgumentException($"This encryption padding {encryptionPadding} does not support with key size < 2048");

                        hashLength = 512;
                        break;
                    }
                default:
                    throw new ArgumentException($"This encryption padding {encryptionPadding} does not support");
            }

            return CalculateMaxDataSize(certificate, hashLength);
        }

        private int CalculateMaxDataSize(X509Certificate2 certificate, int hashLength)
        {
            if (hashLength <= 0)
                throw new ArgumentException($"Parameter {nameof(hashLength)}={hashLength} must be greater then zero");

            return (int)(0.125 * certificate.PrivateKey.KeySize - 0.25 * hashLength - 2); //(_certificate.PrivateKey.KeySize / 8) - 2 * (hashLength / 8) - 2;
        }

        private IEnumerable<T[]> Split<T>(T[] array, int size)
        {
            if (size <= 0)
                throw new ArgumentException($"Parameter {nameof(size)}={size} must be greater then zero");

            for (int i = 0; i < array.Length; i += size)
            {
                int arraySize = array.Length - i < size ? array.Length - i : size;

                var tempArray = new T[arraySize];
                Buffer.BlockCopy(array, i, tempArray, 0, arraySize);

                yield return tempArray;
            }
        }

        private T[] Combine<T>(IEnumerable<T[]> arrays)
        {
            var enumerable = arrays as T[][] ?? arrays.ToArray();
            var resultArray = new T[enumerable.Sum(a => a.Length)];

            int offset = 0;
            foreach (var array in enumerable)
            {
                Buffer.BlockCopy(array, 0, resultArray, offset, array.Length);
                offset += array.Length;
            }

            return resultArray;
        }

        public void Dispose()
        {
            if (_encryptionRSA.IsValueCreated)
                _encryptionRSA?.Value.Dispose();

            _decryptionRSA?.Dispose();
        }
    }
}
