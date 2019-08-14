using System;
using System.Net.Http;
using System.Threading;
using Polly;

namespace _05_http_cb
{
    class Program
    {
        private static string OlReliableUrl = "http://www.mocky.io/v2/5d5356ab2e00005e0081de45";
        private static string FallbackUrl = "http://www.mocky.io/v2/5d542f562f00002b008614b6";

        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var circuitBreakerPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode > 499)
                    .CircuitBreakerAsync(2, TimeSpan.FromSeconds(10));

                var fallbackPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode > 499)
                    .FallbackAsync((cancelToken) => httpClient.GetAsync(FallbackUrl, cancelToken))
                    .WrapAsync(circuitBreakerPolicy);



                while (true)
                {
                    var task = fallbackPolicy.ExecuteAndCaptureAsync(() => httpClient.GetAsync(OlReliableUrl));
                    // execute async code as sync
                    var result = task.GetAwaiter().GetResult();
                    Console.WriteLine($"Outcome: {result.Outcome}");

                    if (result.Outcome == OutcomeType.Successful)
                    {
                        var body = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Console.WriteLine($"{body}");
                    }
                    Console.WriteLine($"Circuit State: {circuitBreakerPolicy.CircuitState}");
                    Thread.Sleep(2000);
                }


            }
        }
    }
}
