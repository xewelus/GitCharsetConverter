using System.Collections.Generic;

namespace Ude.Core
{
	/// <summary>
	/// Multi-byte charsets probers
	/// </summary>
	public class MBCSGroupProber : CharsetProber
	{
		public MBCSGroupProber()
		{
			this.probersByName = new Dictionary<CharsetProber, string>();
			this.probersByName.Add(new UTF8Prober(), "UTF8");

			PROBERS_NUM = this.probersByName.Count;
			this.isActive = new bool[this.probersByName.Count];
			this.probers = new CharsetProber[this.probersByName.Count];
			int i = 0;
			foreach (KeyValuePair<CharsetProber, string> pair in this.probersByName)
			{
				this.probers[i++] = pair.Key;
			}

			this.Reset();
		}

		private static int PROBERS_NUM; // = 7;
		private int activeNum;
		private int bestGuess;
		private readonly bool[] isActive; // = new bool[PROBERS_NUM];
		private readonly CharsetProber[] probers; // = new CharsetProber[PROBERS_NUM];
		private readonly Dictionary<CharsetProber, string> probersByName;

		public override string GetCharsetName()
		{
			if (this.bestGuess == -1)
			{
				this.GetConfidence();
				if (this.bestGuess == -1) this.bestGuess = 0;
			}
			return this.probers[this.bestGuess].GetCharsetName();
		}

		public override void Reset()
		{
			this.activeNum = 0;
			for (int i = 0; i < this.probers.Length; i++)
			{
				if (this.probers[i] != null)
				{
					this.probers[i].Reset();
					this.isActive[i] = true;
					++this.activeNum;
				}
				else
				{
					this.isActive[i] = false;
				}
			}
			this.bestGuess = -1;
			this.state = ProbingState.Detecting;
		}

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			// do filtering to reduce load to probers
			byte[] highbyteBuf = new byte[len];
			int hptr = 0;
			//assume previous is not ascii, it will do no harm except add some noise
			bool keepNext = true;
			int max = offset + len;

			for (int i = offset; i < max; i++)
			{
				if ((buf[i] & 0x80) != 0)
				{
					highbyteBuf[hptr++] = buf[i];
					keepNext = true;
				}
				else
				{
					//if previous is highbyte, keep this even it is a ASCII
					if (keepNext)
					{
						highbyteBuf[hptr++] = buf[i];
						keepNext = false;
					}
				}
			}

			ProbingState st = ProbingState.NotMe;

			for (int i = 0; i < this.probers.Length; i++)
			{
				if (!this.isActive[i])
					continue;
				st = this.probers[i].HandleData(highbyteBuf, 0, hptr);
				if (st == ProbingState.FoundIt)
				{
					this.bestGuess = i;
					this.state = ProbingState.FoundIt;
					break;
				}
				if (st == ProbingState.NotMe)
				{
					this.isActive[i] = false;
					this.activeNum--;
					if (this.activeNum <= 0)
					{
						this.state = ProbingState.NotMe;
						break;
					}
				}
			}
			return this.state;
		}

		public override float GetConfidence()
		{
			float bestConf = 0.0f;
			float cf = 0.0f;

			if (this.state == ProbingState.FoundIt)
			{
				return 0.99f;
			}
			if (this.state == ProbingState.NotMe)
			{
				return 0.01f;
			}
			for (int i = 0; i < PROBERS_NUM; i++)
			{
				if (!this.isActive[i])
					continue;
				cf = this.probers[i].GetConfidence();
				if (bestConf < cf)
				{
					bestConf = cf;
					this.bestGuess = i;
				}
			}
			return bestConf;
		}

		public override void DumpStatus()
		{
			float cf;
			this.GetConfidence();
			for (int i = 0; i < PROBERS_NUM; i++)
			{
				if (!this.isActive[i])
				{
				}
				else
				{
					cf = this.probers[i].GetConfidence();
				}
			}
		}
	}
}