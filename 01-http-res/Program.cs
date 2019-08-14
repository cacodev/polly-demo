using System;
using System.Net.Http;
using Polly;
using Polly.Retry;

namespace _01_http_res
{
    class Program
    {
        private static string OlReliableUrl = "http://www.mocky.io/v2/5d5356ab2e00005e0081de45";
        static void Main(string[] args)
        {
            using(var httpClient = new HttpClient())
            {
                var policyResult = Policy
                    .HandleResult<HttpResponseMessage>(r => (int)r.StatusCode > 499)
                    .WaitAndRetryAsync(3,
                    retryAttempt =>
                    {
                        var time = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2);
                        return time;
                    },
                    onRetry: (response, time, attempt, context) =>
                    {
                        Console.WriteLine($"Attempt: {attempt} | Exception: {response}");
                    })
                    .ExecuteAndCaptureAsync(() => httpClient.GetAsync(OlReliableUrl));
                
                // execute async code as sync
                var result = policyResult.GetAwaiter().GetResult();
                Console.WriteLine($"Outcome: {result.Outcome}");
            }
        }
    }
}
