using System;

namespace Cryptography.Wrappers
{
    /// <summary>
    /// Represents interface for implementing encryptors.
    /// </summary>
    public interface IEncryptor : IDisposable
    {
        /// <summary>
        /// Decrypt encrypted data using a specific encryption algorithm
        /// </summary>
        /// <param name="encryptedData">Encrypted data for decryption</param>
        /// <returns>Decrypted data</returns>
        byte[] Decrypt(byte[] encryptedData);

        /// <summary>
        /// Encrypt data using a specific encryption algorithm
        /// </summary>
        /// <param name="data">Data for encryption</param>
        /// <returns>Encrypted data</returns>
        byte[] Encrypt(byte[] data);
    }
}
