using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptography.Wrappers.Certificates
{
    public class ThumbprintCertificateInfo : ICertificateInfo
    {
        private string _thumbprint;

        /// <summary>
        /// Certificate thumbprint
        /// </summary>
        public string Thumbprint
        {
            get => _thumbprint;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException($"{nameof(Thumbprint)} can not be null or empty");

                _thumbprint = Transform(value);
            }
        }

        public StoreLocation Location { get; set; }

        public StoreName StoreName { get; set; }

        public ThumbprintCertificateInfo(string thumbprint, StoreLocation location = StoreLocation.CurrentUser, StoreName storeName = StoreName.My)
        {
            Thumbprint = thumbprint;
            Location = location;
            StoreName = storeName;
        }

        public IEnumerable<SearchCriteria> GetFilters()
        {
            return new[]
            {
                new SearchCriteria(X509FindType.FindByTimeValid, DateTime.Now),
                new SearchCriteria(X509FindType.FindByThumbprint, Thumbprint)
            };
        }

        private string Transform(string value)
        {
            value = value.Replace(" ", string.Empty);

            //Данные спецсимволы возникают при ручном копировании thumbprint из mmc
            value = value.Replace("\u200e", string.Empty);
            value = value.Replace("\u200f", string.Empty);

            return value;
        }
    }
}
