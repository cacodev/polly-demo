using System;
using System.Threading.Tasks;
using System.Net.Http;
using Polly;

namespace _02_http_fallback
{
    class Program
    {
        private static string OlReliableUrl = "http://www.mocky.io/v2/5d5356ab2e00005e0081de45";
        private static string FallbackUrl = "http://www.mocky.io/v2/5d542f562f00002b008614b6";

        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                var policyResult = Policy
                    .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode > 499)
                    .FallbackAsync((cancelToken) => httpClient.GetAsync(FallbackUrl, cancelToken))
                    .ExecuteAndCaptureAsync(() => httpClient.GetAsync(OlReliableUrl));

                // execute async code as sync
                var result = policyResult.GetAwaiter().GetResult();
                Console.WriteLine($"Outcome: {result.Outcome}");

                var body = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine($"{body}");
            }
        }
    }
}
