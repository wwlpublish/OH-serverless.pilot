using System.Collections.Generic;
using ServerlessOpenhack.Models;

namespace ServerlessOpenhack.Services
{
    public class RecordListService
    {
        public static List<Distributor> GetWholesaleDistributorList = new List<Distributor>() {
            new Distributor()
            {
                locationId = "AAA111",
                locationName = "Contoso Suites",
                locationAddress = "123 Wholesale Road",
                locationPostCode = 98112
            },
            new Distributor(){
                locationId = "BBB222",
                locationName = "Northwind Traders",
                locationAddress = "456 Foodcenter Lane",
                locationPostCode = 98101
            },
            new Distributor(){
                locationId = "CCC333",
                locationName = "VanArsdel Ltd.",
                locationAddress = "789 FE Road",
                locationPostCode = 98052
            },
            new Distributor(){
                locationId = "DDD444",
                locationName = "Wide World Importers",
                locationAddress = "645 Roosevelt Avenue",
                locationPostCode = 98121
            }
        };

        public static List<Distributor> GetPointDistributorList = new List<Distributor>() {
            new Distributor(){
                locationId = "FFF666",
                locationName = "Alpine Ski House",
                locationAddress = "10 Scott Road",
                locationPostCode = 98133
            },
            new Distributor(){
                locationId = "GGG777",
                locationName = "Bellows College",
                locationAddress = "8977 FE Road",
                locationPostCode = 98101
            },
            new Distributor(){
                locationId = "HHH888",
                locationName = "Fabrikam Residences",
                locationAddress = "545 Main Street",
                locationPostCode = 98052
            },
            new Distributor(){
                locationId = "III999",
                locationName = "Fourth Coffee",
                locationAddress = "32108 18th Street",
                locationPostCode = 98112
            },
            new Distributor(){
                locationId = "JJJ000",
                locationName = "Liberty's Delightful Sinful Bakery & Cafe",
                locationAddress = "441 36th Street",
                locationPostCode = 98133
            },
            new Distributor(){
                locationId = "KKK111",
                locationName = "Margie's Travel",
                locationAddress = "675 Tyler Place",
                locationPostCode = 98112
            },
            new Distributor(){
                locationId = "LLL222",
                locationName = "Munson's Pickles and Preserves Farm",
                locationAddress = "3453 99th Place",
                locationPostCode = 98121
            },
            new Distributor(){
                locationId = "MMM333",
                locationName = "Southridge Video",
                locationAddress = "53244 Rooster Lane",
                locationPostCode = 98101
            }
        };

        public static List<IceCreamProduct> GetProductList = new List<IceCreamProduct>(){
            new IceCreamProduct()
            {
                productId = "75542e38-563f-436f-adeb-f426f1dabb5c",
                productName = "Starfruit Explosion",
                productDescription = "This starfruit ice cream is out of this world!",
                unitCost = 3.99,
                ratingDistribution = new List<int>() { 2,3,3,4,4,4,5,5 },
                purchaseCountDistribution = new List<int>() { 1,2,3,4 },
            },
            new IceCreamProduct()
            {
                productId = "e94d85bc-7bd0-44f3-854e-d8cd70348b63",
                productName = "Just Peachy",
                productDescription = "Your taste buds and this ice cream were made for peach other.",
                unitCost = 4.99,
                ratingDistribution = new List<int>() { 2,3,3,4,4,5,5,5 },
                purchaseCountDistribution = new List<int>() { 1,2,3,4 },
            },
            new IceCreamProduct()
            {
                productId = "288fd748-ad2b-4417-83b9-7aa5be9cff22",
                productName = "Tropical Mango",
                productDescription = "You know what they say... It takes two.  You.  And this ice cream.",
                unitCost = 5.99,
                ratingDistribution = new List<int>() { 3,4,4,5,5,5,5 },
                purchaseCountDistribution = new List<int>() { 1,2,3,4 },
            },
            new IceCreamProduct()
            {
                productId = "76065ecd-8a14-426d-a4cd-abbde2acbb10",
                productName = "Gone Bananas",
                productDescription = "I'm not sure how appealing banana ice cream really is.",
                unitCost = 4.49,
                ratingDistribution = new List<int>() { 0,1,2 },
                purchaseCountDistribution = new List<int>() { 1,2 },
            },
            new IceCreamProduct()
            {
                productId = "551a9be9-7f1c-447d-83ee-b18f5a6fb018",
                productName = "Matcha Green Tea",
                productDescription = "Green tea ice cream is good for you because it is green.",
                unitCost = 3.99,
                ratingDistribution = new List<int>() { 3,4,4,4,5,5,5 },
                purchaseCountDistribution = new List<int>() { 2,3 },
            },
            new IceCreamProduct()
            {
                productId = "80bab959-ef8b-4ae3-8bf2-e876d77277b6",
                productName = "French Vanilla",
                productDescription = "It's vanilla ice cream.",
                unitCost = 2.99,
                ratingDistribution = new List<int>() { 3,3,4,5 },
                purchaseCountDistribution = new List<int>() { 2,3,4,5,6 },
            },
            new IceCreamProduct()
            {
                productId = "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
                productName = "Truly Orange-inal",
                productDescription = "Made from concentrate.",
                unitCost = 3.49,
                ratingDistribution = new List<int>() { 3,3,4,5 },
                purchaseCountDistribution = new List<int>() { 1,2,3 },
            },
            new IceCreamProduct()
            {
                productId = "65ab124a-9b2c-4294-a52d-18839364ef15",
                productName = "Durian Durian",
                productDescription = "Smells suspect but tastes... also suspect.",
                unitCost = 8.99,
                ratingDistribution = new List<int>() { 0,0,1 },
                purchaseCountDistribution = new List<int>() { 1 },
            },
            new IceCreamProduct()
            {
                productId = "e4e7068e-500e-4a00-8be4-630d4594735b",
                productName = "It's Grape!",
                productDescription = "Unraisinably good ice cream.",
                unitCost = 3.99,
                ratingDistribution = new List<int>() { 0,1,2,3,4 },
                purchaseCountDistribution = new List<int>() { 1,2 },
            },
            new IceCreamProduct()
            {
                productId = "0f5a0fe8-4506-4332-969e-699a693334a8",
                productName = "Beer",
                productDescription = "Hey this isn't ice cream!",
                unitCost = 15.99,
                ratingDistribution = new List<int>() { 4,5,5 },
                purchaseCountDistribution = new List<int>() { 1,2,3,4 },
            }
        };

        public static List<User> GetUserList = new List<User> {
            new User()
            {
                UserId = "cc5581ff-6be1-4418-a8d8-55a29c24b995",
                Username = "garry.thornburg",
                FullName = "Garry Thornburg"},
            new User()
            {
                UserId = "6dd3bb49-a5be-41ca-9dac-3b995450f2db",
                Username = "kayla.cobb",
                FullName = "Kayla Cobb"},
            new User()
            {
                UserId = "ed414804-ed3d-4ec3-a283-f94ee86f3e23",
                Username = "edna.waters",
                FullName = "Edna Waters"},
            new User()
            {
                UserId = "d1f80b77-040f-4ec8-a833-90b18da70337",
                Username = "chester.furlong",
                FullName = "Chester Furlong"},
            new User()
            {
                UserId = "cc20a6fb-a91f-4192-874d-132493685376",
                Username = "doreen.riddle",
                FullName = "Doreen Riddle"}
        };

        public static List<string> GetVeryNegativeSentenceList = new List<string>()
        {
            "Seriously nasty stuff.",
            "Worst ice cream I've had in my life.",
            "I'd rather eat brussels sprouts.  BRUSSELS SPROUTS.  RAW.  Terrible ice cream.",
            "I refuse to even try this ice cream again.",
            "What is with all these gross flavors?",
            "Bad bad ice cream. NEXT!",
        };

        public static List<string> GetNegativeSentenceLList = new List<string>()
        {
            "Not a fan of this flavor.",
            "These people need to go back to culinary school.",
            "This is definitely some weird ice cream.",
            "Objectively bad ice cream.",
            "I don't think I'd recommend this flavor to my friends.",
            "I was forced to finish this because they were watching me.  Never again.",
        };

        public static List<string> GetNeutralSentenceList = new List<string>()
        {
            "All in all... not bad.",
            "No epiphany was had but it was alright.",
            "Meh.",
            "I've had better.",
            "Probably overpriced for what it is.",
            "Some more thought needs to go into this ice cream."
        };

        public static List<string> GetPositiveSentenceList = new List<string>()
        {
            "Loved the flavor.",
            "I'd eat this every day.",
            "Why eat vegetables when you can have this ice cream?",
            "Lovely texture.",
            "Some of the best ice cream I've ever had.",
            "I'd love some more!",
            "I'd sell my soul for a lifetime supply of this beautiful, lovely confection.",
            "I love how daring they were with the flavors!",
            "So fresh.",
            "High quality ice cream from high quality people.",
            "I'll be back for more."
        };
    }
}