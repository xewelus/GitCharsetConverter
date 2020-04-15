using System;

namespace Ude.Core
{
	public abstract class SequenceModel
	{
		public SequenceModel(
			byte[] charToOrderMap,
			byte[] precedenceMatrix,
			float typicalPositiveRatio,
			bool keepEnglishLetter,
			String charsetName)
		{
			this.charToOrderMap = charToOrderMap;
			this.precedenceMatrix = precedenceMatrix;
			this.typicalPositiveRatio = typicalPositiveRatio;
			this.keepEnglishLetter = keepEnglishLetter;
			this.charsetName = charsetName;
		}

		protected String charsetName;

		// [256] table use to find a char's order
		protected byte[] charToOrderMap;

		// not used            
		protected bool keepEnglishLetter;

		// [SAMPLE_SIZE][SAMPLE_SIZE] table to find a 2-char sequence's 
		// frequency        
		protected byte[] precedenceMatrix;

		// freqSeqs / totalSeqs
		protected float typicalPositiveRatio;

		public float TypicalPositiveRatio
		{
			get
			{
				return this.typicalPositiveRatio;
			}
		}

		public bool KeepEnglishLetter
		{
			get
			{
				return this.keepEnglishLetter;
			}
		}

		public string CharsetName
		{
			get
			{
				return this.charsetName;
			}
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