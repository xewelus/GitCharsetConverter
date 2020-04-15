namespace Ude.Core
{
	public class EscCharsetProber : CharsetProber
	{
		public EscCharsetProber()
		{
			this.codingSM = new CodingStateMachine[CHARSETS_NUM];
			this.codingSM[0] = new CodingStateMachine(new HZSMModel());
			this.codingSM[1] = new CodingStateMachine(new ISO2022CNSMModel());
			this.codingSM[2] = new CodingStateMachine(new ISO2022JPSMModel());
			this.codingSM[3] = new CodingStateMachine(new ISO2022KRSMModel());
			this.Reset();
		}

		private const int CHARSETS_NUM = 4;
		int activeSM;
		private readonly CodingStateMachine[] codingSM;
		private string detectedCharset;

		public override void Reset()
		{
			this.state = ProbingState.Detecting;
			for (int i = 0; i < CHARSETS_NUM; i++) this.codingSM[i].Reset();
			this.activeSM = CHARSETS_NUM;
			this.detectedCharset = null;
		}

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			int max = offset + len;

			for (int i = offset; i < max && this.state == ProbingState.Detecting; i++)
			{
				for (int j = this.activeSM - 1; j >= 0; j--)
				{
					// byte is feed to all active state machine
					int codingState = this.codingSM[j].NextState(buf[i]);
					if (codingState == SMModel.ERROR)
					{
						// got negative answer for this state machine, make it inactive
						this.activeSM--;
						if (this.activeSM == 0)
						{
							this.state = ProbingState.NotMe;
							return this.state;
						}
						if (j != this.activeSM)
						{
							CodingStateMachine t = this.codingSM[this.activeSM];
							this.codingSM[this.activeSM] = this.codingSM[j];
							this.codingSM[j] = t;
						}
					}
					else if (codingState == SMModel.ITSME)
					{
						this.state = ProbingState.FoundIt;
						this.detectedCharset = this.codingSM[j].ModelName;
						return this.state;
					}
				}
			}
			return this.state;
		}

		public override string GetCharsetName()
		{
			return this.detectedCharset;
		}

		public override float GetConfidence()
		{
			return 0.99f;
		}
	}
}