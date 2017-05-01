using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AmazonTracker.ProductScraper;
using System.IO;
using CsvHelper;

namespace ConsoleApplication1
{
  class Program
  {
    static void Main(string[] args)
    {
            var inputListPath = @"C:\temp\MasterList.csv";
            var inputProducts = new List<ProductInput>();
            using (var reader = File.OpenText(inputListPath))
            {
                var csv = new CsvReader(reader);
                inputProducts = csv.GetRecords<ProductInput>().ToList();
            }

            if (!args.Any(a => a == "-all"))
            {
                inputProducts = inputProducts.Where(p => p.ASIN == args[0]).ToList();   
            }
            
            foreach (var product in inputProducts)
            {
                Console.WriteLine($"Getting info for product: {product.ASIN}...........");

                try
                {
                    var result = ScraperOperaionts.getProductInfoFromAsin(product.ASIN);
                    Console.WriteLine($"ASIN: {result.ASIN},  Price: {(result.Price.IsSome() ? result.Price.Value : -99)}, Sold By: {result.BuyBoxSeller}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed Parsing for {product.ASIN}");
                    Console.ResetColor();
                }
                
            }

            Console.ReadLine();
    }
  }
}
