using System.IO;
using contract;
using stream_forwarder;
using SharpCompress.Reader;
using System;
using SharpCompress.Common;

namespace stream_receiver
{
    public class StreamReceiver : Streaming
    {
        public void ReceiveStream(Stream stream)
        {
            System.Console.WriteLine("received stream...");

            var teeStream = new TeeInputStream(stream, File.OpenWrite("recevied-archive-file.rar"), true);
            DecopmressStream(teeStream);
            teeStream.Close();
            System.Console.WriteLine("done writing stream to disk!");
        }

        private void DecopmressStream(Stream stream)
        {
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.FilePath);
                    reader.WriteEntryToDirectory(@".", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
        }
    }
}
