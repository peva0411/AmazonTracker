using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication1
{
  class Program
  {
    static void Main(string[] args)
    {
      using (var client = new HttpClient())
      {
        var test = client.GetStringAsync("http://google.com").Result;

        Console.WriteLine($"Result: {test.Length}");
        Console.ReadLine();
      }

    }
  }
}
