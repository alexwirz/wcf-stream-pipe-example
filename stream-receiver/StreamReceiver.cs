using System;
using System.IO;
using System.Threading.Tasks;
using contract;
using SharpCompress.Common;
using SharpCompress.Reader;
using stream_forwarder;

namespace stream_receiver
{
    public class StreamReceiver : Streaming
    {
        public void ReceiveStream(Stream stream)
        {
            System.Console.WriteLine("received stream...");
            Directory.CreateDirectory("receiver");
            var teeStream = new TeeInputStream(stream, File.OpenWrite("recevied-archive-file.rar"), true);
            DecompressStream(teeStream);
            teeStream.Close();
            System.Console.WriteLine("done writing stream to disk!");
        }

        private void DecompressStream(Stream stream)
        {
            var decompressPipe = new PipeStream();
            var sourceStreamReadingTask = Task.Factory.StartNew(() => stream.CopyTo(decompressPipe));
            var reader = ReaderFactory.Open(decompressPipe);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.FilePath);
                    reader.WriteEntryToDirectory(@"receiver", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
            sourceStreamReadingTask.Wait();
        }
    }
}
