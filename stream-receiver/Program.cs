using System;
using System.ServiceModel;

namespace stream_receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(StreamReceiver));
            serviceHost.Open();
            Console.WriteLine("stream receiver is up and running. hit <ENTER> to close...");
            Console.ReadLine();
            serviceHost.Close();
        }
    }
}
