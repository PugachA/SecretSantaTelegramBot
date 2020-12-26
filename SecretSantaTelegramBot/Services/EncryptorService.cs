using Cryptography.Wrappers;
using Cryptography.Wrappers.Certificates;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace SecretSantaTelegramBot.Services
{
    public class EncryptorService : IEncryptor
    {
        private readonly RsaEncryptor _rsaEncryptor;

        public EncryptorService(IOptions<ThumbprintCertificateInfo> certificateInfo)
        {
            if (certificateInfo is null || certificateInfo.Value is null)
                throw new ArgumentNullException($"Parametr {nameof(certificateInfo)} can not be null");

            var certificate = CertificateExtractor.FindCertificate(certificateInfo.Value);

            if (certificate is null)
                throw new ArgumentNullException($"Can not find certificate {JsonConvert.SerializeObject(certificateInfo.Value)}");

            _rsaEncryptor = new RsaEncryptor(certificate);
        }

        public byte[] Encrypt(byte[] data)
        {
            return _rsaEncryptor.Encrypt(data);
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            return _rsaEncryptor.Decrypt(encryptedData);
        }

        public void Dispose()
        {
            _rsaEncryptor?.Dispose();
        }
    }
}
