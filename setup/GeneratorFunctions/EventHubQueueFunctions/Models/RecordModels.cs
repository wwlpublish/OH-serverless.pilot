using System;
using System.Collections.Generic;

namespace ServerlessOpenhack.Models
{
    public class EventDto
    {
        public HeaderDto header { get; set; }
        public OrderDetailsDto[] details { get; set; }
    }

    public class HeaderDto
    {
        public string salesNumber { get; set; }
        public DateTime dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public string locationPostcode { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string receiptUrl { get; set; }
    }

    public class OrderDetailsDto
    {
        public string productId { get; set; }
        public string quantity { get; set; }
        public string unitCost { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
    }

    public class IceCreamProduct
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public double unitCost { get; set; }
        public List<int> ratingDistribution { get; set; }
        public List<int> purchaseCountDistribution { get; set; }
    }

    public class Distributor
    {
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public int locationPostCode { get; set; }
    }

    public class Order
    {
        public string poNumber { get; set; }
        public DateTime dateTime { get; set; }
        public Distributor distributor { get; set; }
        public List<OrderLineItem> lineItems { get; set; }
    }

    public class OrderHeaderDetail
    {
        public string poNumber { get; set; }
        public DateTime dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public int locationPostCode { get; set; }
        public double totalCost { get; set; }
        public double totalTax { get; set; }
    }

    public class OrderLineItem
    {
        public string poNumber { get; set; }
        public string productId { get; set; }
        public int quantity { get; set; }
        public double unitCost { get; set; }
        public double totalCost { get; set; }
        public double totalTax { get; set; }
        public IceCreamProduct product { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
    }
}