using System;
using System.ServiceModel;

namespace stream_forwarder
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(StreamForwarder));
            serviceHost.Open();
            Console.WriteLine("stream forwarder is up and running. hit <ENTER> to close...");
            Console.ReadLine();
            serviceHost.Close();
        }
    }
}
