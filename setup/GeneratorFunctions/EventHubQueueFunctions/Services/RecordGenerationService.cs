using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using ServerlessOpenhack.Models;

namespace ServerlessOpenhack.Services
{
    public static class RecordGenerationService
    {
        public static List<IceCreamProduct> ProductList { get => RecordListService.GetProductList; set => RecordListService.GetProductList = value; }

        public static List<EventDto> GenerateEvents(int numberEvents)
        {
            // generate specific orders
            var fakeProductDto = new Faker<OrderDetailsDto>()
                .StrictMode(true)
                .RuleFor(p => p.productId, f => f.PickRandom(ProductList).productId)
                .RuleFor(p => p.productName, (f, p) => ProductList.FirstOrDefault(e => e.productId == p.productId).productName)
                .RuleFor(p => p.productDescription, (f, p) => ProductList.FirstOrDefault(e => e.productId == p.productId).productDescription)
                .RuleFor(p => p.unitCost, (f, p) => ProductList.FirstOrDefault(e => e.productId == p.productId).unitCost.ToString())
                .RuleFor(p => p.quantity, (f, p) => f.PickRandom(ProductList.FirstOrDefault(e => e.productId == p.productId).purchaseCountDistribution).ToString())
                .RuleFor(p => p.totalCost, (f, p) => (Int32.Parse(p.quantity) * decimal.Parse(p.unitCost)).ToString())
                .RuleFor(p => p.totalTax, (f, p) => (double.Parse(p.totalCost) * .1).ToString());

            // generate header
            var fakeHeaderDto = new Faker<HeaderDto>()
                .RuleFor(p => p.salesNumber, f => f.Random.Uuid().ToString())
                .RuleFor(p => p.dateTime, f => f.Date.Recent())
                .RuleFor(p => p.locationId, f => f.PickRandom(RecordListService.GetPointDistributorList).locationId)
                .RuleFor(p => p.locationName, (f, p) => RecordListService.GetPointDistributorList.FirstOrDefault(e => e.locationId == p.locationId).locationName)
                .RuleFor(p => p.locationAddress, (f, p) => RecordListService.GetPointDistributorList.FirstOrDefault(e => e.locationId == p.locationId).locationAddress)
                .RuleFor(p => p.locationPostcode, (f, p) => RecordListService.GetPointDistributorList.FirstOrDefault(e => e.locationId == p.locationId).locationPostCode.ToString())
                .RuleFor(p => p.receiptUrl, f => null);

            // generate whole event
            var fakeEventDto = new Faker<EventDto>()
                .RuleFor(p => p.details, f => fakeProductDto.Generate(f.Random.Number(1, 10)).Distinct(new OrderComparisonService()).ToArray())
                .RuleFor(p => p.header, f => fakeHeaderDto.Generate());

            var generatedEvents = fakeEventDto.Generate(numberEvents);

            // set totalCost, totalTax, and add receiptUrl in header
            Random random = new Random();
            foreach (var g in generatedEvents)
            {
                // 20% of the time, add a receipt url
                double randomNumber = random.NextDouble();
                if (randomNumber <= 0.2)
                {
                    g.header.receiptUrl = Environment.GetEnvironmentVariable("RECEIPT_URL");
                }
                g.header.totalCost = g.details.Sum(order => decimal.Parse(order.totalCost)).ToString();
                g.header.totalTax = Math.Round(g.details.Sum(order => decimal.Parse(order.totalTax)), 2).ToString();

                foreach (var productOrder in g.details)
                {
                    productOrder.totalTax = Math.Round(decimal.Parse(productOrder.totalTax), 2).ToString();
                }
            }

            return generatedEvents;
        }

        public static List<Order> GenerateCSVs(int numOrders)
        {
            Faker<Order> fakeOrderDto = new Faker<Order>()
                .RuleFor(o => o.distributor, f => f.PickRandom(RecordListService.GetWholesaleDistributorList))
                .RuleFor(o => o.dateTime, f => f.Date.Between(DateTime.Now.AddMonths(-1), DateTime.Now))
                .RuleFor(o => o.poNumber, f => f.Random.Replace("???###"))
                .RuleFor(o => o.lineItems, (f, o) => GenerateListItems(o.poNumber));

            return fakeOrderDto.Generate(numOrders);
        }

        public static List<OrderLineItem> GenerateListItems(string poNumber)
        {
            Random r = new Random();
            int numItems = r.Next(1, 5);

            Faker<OrderLineItem> fakeItemDto = new Faker<OrderLineItem>()
                .RuleFor(i => i.poNumber, poNumber)
                .RuleFor(i => i.product, f => f.PickRandom(RecordListService.GetProductList))
                .RuleFor(i => i.productId, (f, i) => RecordListService.GetProductList.FirstOrDefault(e => e.productId == i.product.productId).productId)
                .RuleFor(i => i.unitCost, (f, i) => RecordListService.GetProductList.FirstOrDefault(e => e.productId == i.product.productId).unitCost)
                .RuleFor(i => i.quantity, (f, i) => f.PickRandom(i.product.purchaseCountDistribution) * 5)
                .RuleFor(i => i.totalCost, (f, i) => i.quantity * i.unitCost)
                .RuleFor(i => i.totalTax, (f, i) => i.totalCost * .1);

            return fakeItemDto.Generate(numItems);
        }

        public static RatingDto GenerateRating()
        {
            Faker<RatingDto> fakeRatingDto = new Faker<RatingDto>()
                .RuleFor(i => i.userId, f => f.PickRandom(RecordListService.GetUserList).UserId)
                .RuleFor(i => i.productId, f => f.PickRandom(RecordListService.GetProductList).productId)
                .RuleFor(i => i.locationName, f => f.PickRandom(RecordListService.GetPointDistributorList).locationName)
                .RuleFor(i => i.rating, (f, i) => f.PickRandom(RecordListService.GetProductList.FirstOrDefault(p => p.productId == i.productId).ratingDistribution))
                .RuleFor(i => i.userNotes, (f, i) => GenerateUserNotes(i.rating));

            return fakeRatingDto.Generate(1)[0];
        }

        public static string GenerateUserNotes(int rating)
        {
            Random rnd = new Random();
            IEnumerable<string> notesList;
            // very negative rating
            if (rating == 0)
            {
                notesList = RecordListService.GetVeryNegativeSentenceList.OrderBy(x => rnd.Next()).Take(rnd.Next(1, 3));
            }
            // bad rating
            else if (rating < 3)
            {
                notesList = RecordListService.GetNegativeSentenceLList.OrderBy(x => rnd.Next()).Take(rnd.Next(1, 3));
            }
            // neutral rating
            else if (rating == 3)
            {
                notesList = RecordListService.GetNeutralSentenceList.OrderBy(x => rnd.Next()).Take(rnd.Next(1, 3));
            }
            // positive rating
            else
            {
                notesList = RecordListService.GetPositiveSentenceList.OrderBy(x => rnd.Next()).Take(rnd.Next(1, 3));
            }

            return String.Join(" ", notesList);
        }

    }
}
