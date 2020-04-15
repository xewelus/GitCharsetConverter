namespace Ude.Core
{
	public class EUCTWProber : CharsetProber
	{
		public EUCTWProber()
		{
			this.codingSM = new CodingStateMachine(new EUCTWSMModel());
			this.distributionAnalyser = new EUCTWDistributionAnalyser();
			this.Reset();
		}

		private readonly CodingStateMachine codingSM;
		private readonly EUCTWDistributionAnalyser distributionAnalyser;
		private readonly byte[] lastChar = new byte[2];

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			int codingState;
			int max = offset + len;

			for (int i = 0; i < max; i++)
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
						this.distributionAnalyser.HandleOneChar(this.lastChar, 0, charLen);
					}
					else
					{
						this.distributionAnalyser.HandleOneChar(buf, i - 1, charLen);
					}
				}
			}
			this.lastChar[0] = buf[max - 1];

			if (this.state == ProbingState.Detecting)
				if (this.distributionAnalyser.GotEnoughData() && this.GetConfidence() > SHORTCUT_THRESHOLD)
					this.state = ProbingState.FoundIt;
			return this.state;
		}

		public override string GetCharsetName()
		{
			return "EUC-TW";
		}

		public override void Reset()
		{
			this.codingSM.Reset();
			this.state = ProbingState.Detecting;
			this.distributionAnalyser.Reset();
		}

		public override float GetConfidence()
		{
			return this.distributionAnalyser.GetConfidence();
		}
	}
}