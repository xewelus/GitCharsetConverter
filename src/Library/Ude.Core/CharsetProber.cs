using System.IO;

namespace Ude.Core
{
	public enum ProbingState
	{
		Detecting = 0, // no sure answer yet, but caller can ask for confidence
		FoundIt = 1, // positive answer
		NotMe = 2 // negative answer 
	}

	public abstract class CharsetProber
	{
		protected const float SHORTCUT_THRESHOLD = 0.95F;

		// ASCII codes
		private const byte SPACE = 0x20;
		private const byte CAPITAL_A = 0x41;
		private const byte CAPITAL_Z = 0x5A;
		private const byte SMALL_A = 0x61;
		private const byte SMALL_Z = 0x7A;

		protected ProbingState state;

		/// <summary>
		/// Feed data to the prober
		/// </summary>
		/// <param name="buf">a buffer</param>
		/// <param name="offset">offset into buffer</param>
		/// <param name="len">number of bytes available into buffer</param>
		/// <returns>
		/// A <see cref="ProbingState"/>
		/// </returns>
		public abstract ProbingState HandleData(byte[] buf, int offset, int len);

		/// <summary>
		/// Reset prober state
		/// </summary>
		public abstract void Reset();

		public abstract string GetCharsetName();

		public abstract float GetConfidence();

		public virtual void DumpStatus()
		{
		}

		protected static byte[] FilterWithoutEnglishLetters(byte[] buf, int offset, int len)
		{
			byte[] result;

			using (MemoryStream ms = new MemoryStream(buf.Length))
			{
				bool meetMSB = false;
				int max = offset + len;
				int prev = offset;
				int cur = offset;

				while (cur < max)
				{
					byte b = buf[cur];

					if ((b & 0x80) != 0)
					{
						meetMSB = true;
					}
					else if (b < CAPITAL_A || (b > CAPITAL_Z && b < SMALL_A)
					                       || b > SMALL_Z)
					{
						if (meetMSB && cur > prev)
						{
							ms.Write(buf, prev, cur - prev);
							ms.WriteByte(SPACE);
							meetMSB = false;
						}
						prev = cur + 1;
					}
					cur++;
				}

				if (meetMSB && cur > prev)
					ms.Write(buf, prev, cur - prev);
				ms.SetLength(ms.Position);
				result = ms.ToArray();
			}
			return result;
		}
	}
}