using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using contract;
using System;
using SharpCompress.Reader;
using SharpCompress.Common;

namespace stream_forwarder
{
    public class StreamForwarder : Streaming
    {
        public void ReceiveStream(System.IO.Stream stream)
        {
            System.Console.WriteLine("begining forwarding the stream...");

            var netPipe = new PipeStream();
            var forwardingTask = Task.Factory.StartNew(() => ForwardStream(netPipe));

            var outputFileStream = File.OpenWrite(@"forwarder\cached-archive.rar");
            var teeCache = new TeeOutputStream(outputFileStream, netPipe);

            var tee = new TeeInputStream(stream, teeCache);

            DecopmressStream(tee);

            Console.WriteLine("procssing stream ready...");
            teeCache.Flush();
            outputFileStream.Close();
            Console.WriteLine("netPipe flushed...");

            try
            {
                forwardingTask.Wait();
            }
            catch (AggregateException aggregateException) {
                Console.WriteLine(aggregateException);
            }
            System.Console.WriteLine("stream successfully forwarded.");
        }

        private void ForwardStream(Stream stream) {
            var channel = new ChannelFactory<Streaming>("stream-receiver-ep").CreateChannel();
            channel.ReceiveStream(stream);
            Console.WriteLine("channel received stream...");
            stream.Close();
        }

        private void DecopmressStream(Stream stream)
        {
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.FilePath);
                    reader.WriteEntryToDirectory(@"forwarder", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
        }
    }
}
