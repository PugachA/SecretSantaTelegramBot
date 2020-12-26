using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptography.Wrappers.Certificates
{
    public static class CertificateExtractor
    {
        /// <summary>
        /// Find certificate using filters in instanse of <see cref="ICertificateInfo"/>
        /// </summary>
        /// <param name="certificateInfo">Certificate information</param>
        /// <param name="validOnly">Searching only valid ceretificates (Root certificate is trusted)</param>
        /// <returns>Certificate</returns>
        /// <exception cref="CryptographicException">Thrown when more than one client certificates found matching the criteria.</exception>
        public static X509Certificate2 FindCertificate(ICertificateInfo certificateInfo, bool validOnly = true)
        {
            if (certificateInfo is null)
                throw new ArgumentNullException($"Parametr {nameof(certificateInfo)} can not be null");

            X509Store store = new X509Store(certificateInfo.StoreName, certificateInfo.Location);

            try
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection foundCertificates = store.Certificates;

                IEnumerable<SearchCriteria> filters = certificateInfo.GetFilters();

                foreach (SearchCriteria filter in filters)
                    foundCertificates = foundCertificates.Find(filter.FindType, filter.Value, validOnly);

                if (foundCertificates.Count > 1)
                    throw new CryptographicException(string.Format("More than one ({0}) client certificates found matching the criteria. Filters:\r\n{1}.",
                            foundCertificates.Count, string.Join(",\r\n", filters.Select(filter => string.Concat(filter.FindType, ": ", filter.Value)))));

                return foundCertificates?.OfType<X509Certificate2>().FirstOrDefault();
            }
            finally
            {
                store.Close();
            }
        }
    }
}
