using System;
using System.IO;

namespace stream_forwarder
{
	public class TeeInputStream : TeeStream
	{
		public TeeInputStream(Stream inputStream, Stream secondary, bool autoClose = false)
			: base(inputStream, secondary, autoClose)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = Primary.Read(buffer, offset, count);
			try { Secondary.Write(buffer, offset, read); }
			catch (Exception ex) { throw new TeeException(ex); }
			return read;
		}

		public override bool CanRead { get { return true; } }
	}
}