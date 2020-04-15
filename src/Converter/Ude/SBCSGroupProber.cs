using System;
using System.Collections.Generic;

namespace Ude.Core
{
	public sealed class SBCSGroupProber : CharsetProber
	{
		public SBCSGroupProber()
		{
			List<CharsetProber> list = new List<CharsetProber>();
			list.Add(new SingleByteCharSetProber(new Win1251Model()));
			list.Add(new SingleByteCharSetProber(new Ibm866Model()));
			//list.Add(new SingleByteCharSetProber(new Koi8rModel()));

			this.probers = list.ToArray();
			PROBERS_NUM = this.probers.Length;
			this.isActive = new bool[this.probers.Length];

			this.Reset();
		}

		private static int PROBERS_NUM; //; = 13;
		private int activeNum;
		private int bestGuess;
		private readonly bool[] isActive; // = new bool[PROBERS_NUM];
		private readonly CharsetProber[] probers; // = new CharsetProber[PROBERS_NUM];        

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			//apply filter to original buffer, and we got new buffer back
			//depend on what script it is, we will feed them the new buffer 
			//we got after applying proper filter
			//this is done without any consideration to KeepEnglishLetters
			//of each prober since as of now, there are no probers here which
			//recognize languages with English characters.
			byte[] newBuf = FilterWithoutEnglishLetters(buf, offset, len);
			if (newBuf.Length == 0)
				return this.state; // Nothing to see here, move on.

			for (int i = 0; i < PROBERS_NUM; i++)
			{
				if (!this.isActive[i])
					continue;
				ProbingState st = this.probers[i].HandleData(newBuf, 0, newBuf.Length);

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
			switch (this.state)
			{
				case ProbingState.FoundIt:
					return 0.99f; //sure yes
				case ProbingState.NotMe:
					return 0.01f; //sure no
				default:
					for (int i = 0; i < PROBERS_NUM; i++)
					{
						if (!this.isActive[i])
							continue;
						float cf = this.probers[i].GetConfidence();
						if (bestConf < cf)
						{
							bestConf = cf;
							this.bestGuess = i;
						}
					}
					break;
			}
			return bestConf;
		}

		public override void DumpStatus()
		{
			float cf = this.GetConfidence();
			if (CharsetDetector.NeedConsoleLog)
			{
				Console.WriteLine(" SBCS Group Prober --------begin status");
			}
			for (int i = 0; i < PROBERS_NUM; i++)
			{
				if (!this.isActive[i])
				{
					if (CharsetDetector.NeedConsoleLog)
					{
						Console.WriteLine(" inactive: [{0}] (i.e. confidence is too low).", this.probers[i].GetCharsetName());
					}
				}
				else
				{
					this.probers[i].DumpStatus();
				}
			}
			if (CharsetDetector.NeedConsoleLog)
			{
				Console.WriteLine(" SBCS Group found best match [{0}] confidence {1}.", this.probers[this.bestGuess].GetCharsetName(), cf);
			}
		}

		public override void Reset()
		{
			this.activeNum = 0;
			for (int i = 0; i < PROBERS_NUM; i++)
			{
				if (this.probers[i] != null)
				{
					this.probers[i].Reset();
					this.isActive[i] = true;
					this.activeNum++;
				}
				else
				{
					this.isActive[i] = false;
				}
			}
			this.bestGuess = -1;
			this.state = ProbingState.Detecting;
		}

		public override string GetCharsetName()
		{
			//if we have no answer yet
			if (this.bestGuess == -1)
			{
				this.GetConfidence();
				//no charset seems positive
				if (this.bestGuess == -1) this.bestGuess = 0;
			}
			return this.probers[this.bestGuess].GetCharsetName();
		}
	}
}