using System;
using System.IO;

namespace stream_forwarder
{
	public class TeeOutputStream : TeeStream
	{
		public TeeOutputStream(Stream primary, Stream secondary, bool autoClose = false)
			: base(primary, secondary, autoClose)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			try { Secondary.Write(buffer, offset, count); }
			catch (Exception ex) { throw new TeeException(ex); }
			Primary.Write(buffer, offset, count);
		}

		public override bool CanWrite { get { return true; } }
	}
}