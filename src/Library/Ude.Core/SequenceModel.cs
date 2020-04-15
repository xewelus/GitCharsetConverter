namespace Ude.Core
{
	public abstract class SequenceModel
	{
		protected SequenceModel(
			byte[] charToOrderMap,
			byte[] precedenceMatrix,
			float typicalPositiveRatio,
			string charsetName)
		{
			this.charToOrderMap = charToOrderMap;
			this.precedenceMatrix = precedenceMatrix;
			this.TypicalPositiveRatio = typicalPositiveRatio;
			this.CharsetName = charsetName;
		}

		// [256] table use to find a char's order
		private readonly byte[] charToOrderMap;

		// [SAMPLE_SIZE][SAMPLE_SIZE] table to find a 2-char sequence's 
		// frequency        
		private readonly byte[] precedenceMatrix;

		public float TypicalPositiveRatio { get; }

		public string CharsetName
		{
			get;
		}

		public byte GetOrder(byte b)
		{
			return this.charToOrderMap[b];
		}

		public byte GetPrecedence(int pos)
		{
			return this.precedenceMatrix[pos];
		}
	}
}