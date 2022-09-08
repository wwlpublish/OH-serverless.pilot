using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessOpenhack.Models;

namespace ServerlessOpenhack.Services
{
    public static class CSVService
    {
        public static async Task SendBatch(OpenHackTeam team, string firstCSV, string secondCSV, string thirdCSV, TraceWriter log)
        {
            try
            {
                //Connect to the storage account's blob service
                CloudStorageAccount account = CloudStorageAccount.Parse(team.storageAccountConnectionString);
                CloudBlobClient serviceClient = account.CreateCloudBlobClient();

                //Connect to the team's blob container
                CloudBlobContainer container = serviceClient.GetContainerReference(team.blobContainerName);
                await container.CreateIfNotExistsAsync();

                //Upload the CSVs to the blob
                string orderHeaderDetailsFile = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}-OrderHeaderDetails.csv";
                string orderLineItemsFile = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}-OrderLineItems.csv";
                string productInformationFile = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}-ProductInformation.csv";
                CloudBlockBlob blob = container.GetBlockBlobReference(orderHeaderDetailsFile);
                CloudBlockBlob blob2 = container.GetBlockBlobReference(orderLineItemsFile);
                CloudBlockBlob blob3 = container.GetBlockBlobReference(productInformationFile);


                //Send CSVs in random order
                Random r = new Random();
                var blobList = new List<Tuple<CloudBlockBlob, string>>
                {
                    new Tuple<CloudBlockBlob, string>(
                        blob, firstCSV
                    ),
                    new Tuple<CloudBlockBlob, string>(
                        blob2, secondCSV
                    ),
                    new Tuple<CloudBlockBlob, string>(
                        blob3, thirdCSV
                    ),
                };

                while (blobList.Count > 0)
                {
                    // random next tuple
                    var randomNextIndex = r.Next(0, blobList.Count);
                    //log.Info("writing blob " + randomNextIndex.ToString());
                    var blobStringTuple = blobList[randomNextIndex];

                    // send
                    await blobStringTuple.Item1.UploadTextAsync(blobStringTuple.Item2.ToString());

                    blobStringTuple.Item1.Properties.ContentType = "text/csv";
                    await blobStringTuple.Item1.SetPropertiesAsync();

                    // break before delaying if last file was just sent
                    if (blobList.Count == 1) break;
                    await Task.Delay(r.Next(0, 10000));

                    // remove randomly chosen index
                    blobList.RemoveAt(randomNextIndex);
                }

                log.Info("Wrote CSVs for team #" + team.teamTableNumber);
            }
            catch (Exception e)
            {
                log.Info(team.teamTableNumber + " error - " + e.Message);
            }
        }

        public static string CreateFirstCSVContent(List<Order> orders)
        {
            StringBuilder csv = new StringBuilder();

            //Add headers
            string headers = string.Format("ponumber,datetime,locationid,locationname,locationaddress,locationpostcode,totalcost,totaltax");
            csv.AppendLine(headers);

            //Generate header details
            List<OrderHeaderDetail> headerDetails = new List<OrderHeaderDetail>();
            foreach (Order order in orders)
            {
                double cost = 0;
                double tax = 0;
                foreach (OrderLineItem item in order.lineItems)
                {
                    cost += item.totalCost;
                    tax += item.totalTax;
                }
                headerDetails.Add(new OrderHeaderDetail()
                {
                    poNumber = order.poNumber,
                    dateTime = order.dateTime,
                    locationId = order.distributor.locationId,
                    locationName = order.distributor.locationName,
                    locationAddress = order.distributor.locationAddress,
                    locationPostCode = order.distributor.locationPostCode,
                    totalCost = cost,
                    totalTax = tax
                }
                );
            }

            //Add content
            foreach (OrderHeaderDetail detail in headerDetails)
            {
                string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", detail.poNumber, detail.dateTime, detail.locationId, detail.locationName, detail.locationAddress, detail.locationPostCode, detail.totalCost, detail.totalTax);
                csv.AppendLine(newLine);
            }
            return csv.ToString().Trim();
        }

        public static string CreateSecondCSVContent(List<Order> orders)
        {
            StringBuilder csv = new StringBuilder();

            //Add headers
            string headers = string.Format("ponumber,productid,quantity,unitcost,totalcost,totaltax");
            csv.AppendLine(headers);

            //Generate line items
            List<OrderLineItem> lineItems = new List<OrderLineItem>();
            foreach (Order order in orders)
            {
                lineItems.AddRange(order.lineItems);
            }

            //Add content
            foreach (OrderLineItem item in lineItems)
            {
                string newLine = string.Format("{0},{1},{2},{3},{4},{5}", item.poNumber, item.productId, item.quantity, item.unitCost, item.totalCost, item.totalTax);
                csv.AppendLine(newLine);
            }
            return csv.ToString().Trim();
        }

        public static string CreateThirdCSVContent(List<Order> orders)
        {
            StringBuilder csv = new StringBuilder();

            //Add headers
            string headers = string.Format("productid,productname,productdescription");
            csv.AppendLine(headers);

            // Add products that have been ordered
            List<IceCreamProduct> orderedProducts = RecordListService.GetProductList
                .Where(product => orders.Any(order => order.lineItems.Any(lineItem => lineItem.productId == product.productId)))
                .ToList();
            foreach (IceCreamProduct product in orderedProducts)
            {
                string newLine = string.Format("{0},{1},{2}", product.productId, product.productName, product.productDescription);
                csv.AppendLine(newLine);
            }
            return csv.ToString().Trim();
        }
    }
}


