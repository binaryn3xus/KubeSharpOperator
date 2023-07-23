// See https://aka.ms/new-console-template for more information
using System.Net.NetworkInformation;

Console.WriteLine("Starting Job!");

string hostNameOrAddress = "google.com"; // Replace with the host you want to ping
int loopCount = 10;

for(int i = 0; i < loopCount; i++)
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
    Thread.Sleep(10000);
}

