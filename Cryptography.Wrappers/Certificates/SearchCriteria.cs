using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptography.Wrappers.Certificates
{
    public class SearchCriteria
    {
        public X509FindType FindType { get; }
        public object Value { get; }

        public SearchCriteria(X509FindType findType, object value)
        {
            this.FindType = findType;
            this.Value = value;
        }
    }
}
