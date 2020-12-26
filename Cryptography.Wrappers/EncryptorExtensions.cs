using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography.Wrappers
{
    /// <summary>
    /// Extension class supports convert encryption data to different types
    /// </summary>
    public static class EncryptorExtensions
    {
        /// <summary>
        /// Decrypt data from <see cref="byte"/> array to <see cref="string"/> using <see cref="IEncryptor.Decrypt(byte[])"/> and <see cref="Encoding"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="encryptedData">Encrypted data for decryption</param>
        /// <param name="encoding">Encoding for conversion</param>
        /// <returns>Converting to string decrypted data</returns>
        public static string DecryptString(this IEncryptor encryptor, byte[] encryptedData, Encoding encoding)
        {
            encryptedData.ArrayNullOrEmptyValidate(nameof(encryptedData));
            encoding.NullValidate(nameof(encoding));

            var decryptedByteData = encryptor.Decrypt(encryptedData);

            return encoding.GetString(decryptedByteData);
        }

        /// <summary>
        /// Decrypt data from <see cref="string"/> to <see cref="byte"/> array using <see cref="Convert.FromBase64String(string)"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="encryptedData">Encrypted data for decryption</param>
        /// <returns>Decrypted data</returns>
        public static byte[] DecryptFromBase64String(this IEncryptor encryptor, string encryptedData)
        {
            encryptedData.StringNullOrEmptyValidate(nameof(encryptedData));

            var encryptedByteData = Convert.FromBase64String(encryptedData);

            return encryptor.Decrypt(encryptedByteData);
        }

        /// <summary>
        /// Decrypt data from base64 <see cref="string"/> to <see cref="string"/> using <see cref="Convert.FromBase64String(string)"/> and <see cref="Encoding"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="encryptedData">Encrypted base64 data for decryption</param>
        /// <param name="encoding">Encoding for conversion to string</param>
        /// <returns>Decrypted string data</returns>
        public static string DecryptStringFromBase64String(this IEncryptor encryptor, string encryptedData, Encoding encoding)
        {
            encoding.NullValidate(nameof(encoding));

            var decryptedByteData = encryptor.DecryptFromBase64String(encryptedData);

            return encoding.GetString(decryptedByteData);
        }

        /// <summary>
        /// Encrypt data from <see cref="string"/> to <see cref="byte"/> array using <see cref="IEncryptor.Encrypt(byte[])"/> and <see cref="Encoding"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="data">Data for encryption</param>
        /// <param name="encoding">Encoding for conversion</param>
        /// <returns>Encrypted data</returns>
        public static byte[] EncryptString(this IEncryptor encryptor, string data, Encoding encoding)
        {
            data.NullValidate(nameof(data));
            encoding.NullValidate(nameof(encoding));

            var byteData = encoding.GetBytes(data);

            return encryptor.Encrypt(byteData);
        }

        /// <summary>
        /// Encrypt data from <see cref="byte"/> array to <see cref="string"/> using <see cref="Convert.ToBase64String(byte[])"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="data">Data for encryption</param>
        /// <returns>Encrypted base64 string data</returns>
        public static string EncryptToBase64String(this IEncryptor encryptor, byte[] data)
        {
            data.NullValidate(nameof(data));

            var encryptedByteData = encryptor.Encrypt(data);

            return Convert.ToBase64String(encryptedByteData);
        }

        /// <summary>
        /// Encrypt data from <see cref="string"/> to base64 <see cref="string"/> using <see cref="Convert.ToBase64String(byte[])"/> and <see cref="Encoding"/>
        /// </summary>
        /// <param name="encryptor">Specific encryptor</param>
        /// <param name="data">String data for encryption</param>
        /// <param name="encoding">Encoding for conversion <see cref="string"/> to byte[]</param>
        /// <returns>Encrypted base64 string data</returns>
        public static string EncryptStringToBase64String(this IEncryptor encryptor, string data, Encoding encoding)
        {
            encoding.NullValidate(nameof(encoding));

            var byteData = encoding.GetBytes(data);

            return encryptor.EncryptToBase64String(byteData);
        }
    }
}
