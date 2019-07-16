using HomeCinema.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Tests.Helper
{
    public class CustomerComparer : IComparer, IComparer<Customer>
    {
        

        public int Compare(object expected, object actual)
        {
            var lhs = expected as Customer;
            var rhs = actual as Customer;
            if (lhs == null || rhs == null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }

        public int Compare(Customer expected, Customer actual)
        {
            int temp;
            return (temp = expected.Id.CompareTo(actual.Id)) != 0 ? temp : expected.FirstName.CompareTo(actual.FirstName);
        }
    }
}
