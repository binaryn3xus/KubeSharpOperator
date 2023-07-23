// See https://aka.ms/new-console-template for more information
using System.Net.NetworkInformation;
using TestJob.Extensions;

Console.WriteLine("Starting Job!");

string hostNameOrAddress = EnvironmentExtensions.ParseStringFromEnvironmentVariable("HOSTNAME", "google.com"); // Replace with the host you want to ping
int loopCount = EnvironmentExtensions.ParseIntFromEnvironmentVariable("LOOPCOUNT", 10);
int sleepTimeMs = EnvironmentExtensions.ParseIntFromEnvironmentVariable("SLEEPTIME", 10000); ;

Console.WriteLine("Host: " + hostNameOrAddress);
Console.WriteLine("Loop Count: " + loopCount);
Console.WriteLine("Sleep Time (ms): " + sleepTimeMs);

for (int i = 1; i <= loopCount; i++)
{
    using Ping pingSender = new Ping();
    try
    {
        PingReply reply = pingSender.Send(hostNameOrAddress);

        if (reply.Status == IPStatus.Success)
        {
            Console.WriteLine($"Ping to {hostNameOrAddress} was successful.");
            Console.WriteLine($"Roundtrip time: {reply.RoundtripTime}ms");
        }
        else
        {
            Console.WriteLine($"Ping to {hostNameOrAddress} failed with status: {reply.Status}");
        }
    }
    catch (PingException ex)
    {
        Console.WriteLine($"Ping to {hostNameOrAddress} failed with exception: {ex.Message}");
    }
    Thread.Sleep(sleepTimeMs);
}

