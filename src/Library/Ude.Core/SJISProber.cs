namespace Ude.Core
{
	/// <summary>
	/// for S-JIS encoding, observe characteristic:
	/// 1, kana character (or hankaku?) often have hight frequency of appereance
	/// 2, kana character often exist in group
	/// 3, certain combination of kana is never used in japanese language
	/// </summary>
	public class SJISProber : CharsetProber
	{
		public SJISProber()
		{
			this.codingSM = new CodingStateMachine(new SJISSMModel());
			this.distributionAnalyser = new SJISDistributionAnalyser();
			this.contextAnalyser = new SJISContextAnalyser();
			this.Reset();
		}

		private readonly CodingStateMachine codingSM;
		private readonly SJISContextAnalyser contextAnalyser;
		private readonly SJISDistributionAnalyser distributionAnalyser;
		private readonly byte[] lastChar = new byte[2];

		public override string GetCharsetName()
		{
			return "Shift-JIS";
		}

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			int codingState;
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
						this.contextAnalyser.HandleOneChar(this.lastChar, 2 - charLen, charLen);
						this.distributionAnalyser.HandleOneChar(this.lastChar, 0, charLen);
					}
					else
					{
						this.contextAnalyser.HandleOneChar(buf, i + 1 - charLen, charLen);
						this.distributionAnalyser.HandleOneChar(buf, i - 1, charLen);
					}
				}
			}
			this.lastChar[0] = buf[max - 1];
			if (this.state == ProbingState.Detecting)
				if (this.contextAnalyser.GotEnoughData() && this.GetConfidence() > SHORTCUT_THRESHOLD)
					this.state = ProbingState.FoundIt;
			return this.state;
		}

		public override void Reset()
		{
			this.codingSM.Reset();
			this.state = ProbingState.Detecting;
			this.contextAnalyser.Reset();
			this.distributionAnalyser.Reset();
		}

		public override float GetConfidence()
		{
			float contxtCf = this.contextAnalyser.GetConfidence();
			float distribCf = this.distributionAnalyser.GetConfidence();
			return (contxtCf > distribCf ? contxtCf : distribCf);
		}
	}
}