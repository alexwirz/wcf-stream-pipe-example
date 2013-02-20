using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using contract;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace stream_forwarder
{
    public class StreamForwarder : Streaming
    {
        private void TestBinaryReader(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            while (stream.CanRead)
            {
                try
                {
                    var int16 = br.ReadInt16();
                    Console.WriteLine("still reading...");
                }
                catch (EndOfStreamException endOfStreamException)
                {
                    Console.WriteLine("eos!!!");
                }
            }
        }

        public void ReceiveStream(System.IO.Stream stream)
        {
            System.Console.WriteLine("begining forwarding the stream...");
            var netPipe = new PipeStream();
            var forwardingTask = Task.Factory.StartNew(() => ForwardStream(netPipe));

            Directory.CreateDirectory("forwarder");
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

            var decompressPipe = new PipeStream();
            var sourceStreamReadingTask = Task.Factory.StartNew(() => stream.CopyTo(decompressPipe));
            var reader = ReaderFactory.Open(decompressPipe);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.FilePath);
                    reader.WriteEntryToDirectory(@"forwarder", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
            sourceStreamReadingTask.Wait();
        }
    }
}
