using System;
using System.IO;
using System.ServiceModel;
using contract;

namespace stream_sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("sender is up and running. hit <ENTER> to begin the stream stransmission...");
            Console.ReadLine();
            var channel = new ChannelFactory<Streaming>("stream-forwarder-ep").CreateChannel();
            channel.ReceiveStream(File.OpenRead(@"C:\tmp\ltm.rar"));
            Console.WriteLine("the stream was sent. hit <ENTER> to stop the sender...");
            Console.ReadLine();
            (channel as ICommunicationObject).Close();
        }
    }
}
