using System;
using System.Threading.Tasks;
using System.Net.Http;
using Polly;
using System.Threading;

namespace _03_http_timeout
{
    class Program
    {
        private static string OlReliableUrl = "http://www.mocky.io/v2/5d542f562f00002b008614b6?mocky-delay=5000ms";

        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var timeoutPolicy = Policy.TimeoutAsync(2);

                var task = timeoutPolicy.ExecuteAndCaptureAsync(async (token) => await httpClient.GetAsync(OlReliableUrl, token), CancellationToken.None);

                // execute async code as sync
                var result = task.GetAwaiter().GetResult();
                Console.WriteLine($"Outcome: {result.Outcome}");
            }
        }
    }
}
