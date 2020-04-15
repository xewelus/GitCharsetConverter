namespace Ude.Core
{
	// We use gb18030 to replace gb2312, because 18030 is a superset. 
	public class GB18030Prober : CharsetProber
	{
		public GB18030Prober()
		{
			this.lastChar = new byte[2];
			this.codingSM = new CodingStateMachine(new GB18030SMModel());
			this.analyser = new GB18030DistributionAnalyser();
			this.Reset();
		}

		private readonly GB18030DistributionAnalyser analyser;
		private readonly CodingStateMachine codingSM;
		private readonly byte[] lastChar;

		public override string GetCharsetName()
		{
			return "gb18030";
		}

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			int codingState = SMModel.START;
			int max = offset + len;

			for (int i = offset; i < max; i++)
			{
				codingState = this.codingSM.NextState(buf[i]);
				if (codingState == SMModel.ERROR)
				{
					this.state = ProbingState.NotMe;
					break;
				}
				if (codingState == SMModel.ITSME)
				{
					this.state = ProbingState.FoundIt;
					break;
				}
				if (codingState == SMModel.START)
				{
					int charLen = this.codingSM.CurrentCharLen;
					if (i == offset)
					{
						this.lastChar[1] = buf[offset];
						this.analyser.HandleOneChar(this.lastChar, 0, charLen);
					}
					else
					{
						this.analyser.HandleOneChar(buf, i - 1, charLen);
					}
				}
			}

			this.lastChar[0] = buf[max - 1];

			if (this.state == ProbingState.Detecting)
			{
				if (this.analyser.GotEnoughData() && this.GetConfidence() > SHORTCUT_THRESHOLD) this.state = ProbingState.FoundIt;
			}

			return this.state;
		}

		public override float GetConfidence()
		{
			return this.analyser.GetConfidence();
		}

		public override void Reset()
		{
			this.codingSM.Reset();
			this.state = ProbingState.Detecting;
			this.analyser.Reset();
		}
	}
}