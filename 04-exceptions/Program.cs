using System;
using System.Threading.Tasks;
using Polly;

namespace _04_exceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            var policyAgainstCrappyCode = Policy.Handle<Exception>()
                                                    .Retry(5, onRetry: (Exception exception, int attempt) =>
                                                    {
                                                        Console.WriteLine($"Attempt: {attempt} | Exception Msg: {exception.Message}");
                                                    });
                                                    
            var policyResult = policyAgainstCrappyCode.ExecuteAndCapture(() => RandomlyFailingCode());
        }

        static void RandomlyFailingCode()
        {
            var rand = new Random();

            var shouldFail = rand.Next(0, 5) != 0;

            if (shouldFail)
                throw new Exception("Dude it broke");
        }
    }
}
