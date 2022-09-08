using System.Collections.Generic;
using ServerlessOpenhack.Models;

namespace ServerlessOpenhack.Services
{
    public class OrderComparisonService : IEqualityComparer<OrderDetailsDto>
    {
        public bool Equals(OrderDetailsDto x, OrderDetailsDto y)
        {
            return x.productId == y.productId;
        }

        public int GetHashCode(OrderDetailsDto obj)
        {
            return obj.productId.GetHashCode();
        }
    }
}
