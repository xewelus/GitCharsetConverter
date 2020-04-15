namespace Ude.Core
{
	public class UTF8Prober : CharsetProber
	{
		public UTF8Prober()
		{
			this.numOfMBChar = 0;
			this.codingSM = new CodingStateMachine(new UTF8SMModel());
			this.Reset();
		}

		private static readonly float ONE_CHAR_PROB = 0.50f;
		private readonly CodingStateMachine codingSM;
		private int numOfMBChar;

		public override string GetCharsetName()
		{
			return "UTF-8";
		}

		public override void Reset()
		{
			this.codingSM.Reset();
			this.numOfMBChar = 0;
			this.state = ProbingState.Detecting;
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
					if (this.codingSM.CurrentCharLen >= 2) this.numOfMBChar++;
				}
			}

			if (this.state == ProbingState.Detecting)
				if (this.GetConfidence() > SHORTCUT_THRESHOLD)
					this.state = ProbingState.FoundIt;
			return this.state;
		}

		public override float GetConfidence()
		{
			float unlike = 0.99f;
			float confidence = 0.0f;

			if (this.numOfMBChar < 6)
			{
				for (int i = 0; i < this.numOfMBChar; i++)
					unlike *= ONE_CHAR_PROB;
				confidence = 1.0f - unlike;
			}
			else
			{
				confidence = 0.99f;
			}
			return confidence;
		}
	}
}