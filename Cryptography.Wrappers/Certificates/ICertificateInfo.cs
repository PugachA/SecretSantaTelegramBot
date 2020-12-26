using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptography.Wrappers.Certificates
{
    public interface ICertificateInfo
    {
        /// <summary>
        /// Location (storage) for certificate search.
        /// </summary>
        StoreLocation Location { get; }

        /// <summary>
        /// Name of inner storage.
        /// </summary>
        StoreName StoreName { get; }

        /// <summary>
        /// Returns filters for searching certificate. Based on information stored in instance of <see cref="ICertificateInfo"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<SearchCriteria> GetFilters();
    }
}
