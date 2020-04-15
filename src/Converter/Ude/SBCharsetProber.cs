using System;

namespace Ude.Core
{
	public sealed class SingleByteCharSetProber : CharsetProber
	{
		public SingleByteCharSetProber(SequenceModel model)
			: this(model, false, null)
		{
		}

		private SingleByteCharSetProber(SequenceModel model, bool reversed, CharsetProber nameProber)
		{
			this.model = model;
			this.reversed = reversed;
			this.nameProber = nameProber;
			this.Reset();
		}

		private const int SAMPLE_SIZE = 64;
		private const int SB_ENOUGH_REL_THRESHOLD = 1024;
		private const float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;
		private const float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;
		private const int SYMBOL_CAT_ORDER = 250;
		private const int NUMBER_OF_SEQ_CAT = 4;
		private const int POSITIVE_CAT = NUMBER_OF_SEQ_CAT - 1;

		// characters that fall in our sampling range
		int freqChar;

		// char order of last character
		byte lastOrder;

		private readonly SequenceModel model;

		// Optional auxiliary prober for name decision. created and destroyed by the GroupProber
		readonly CharsetProber nameProber;

		// true if we need to reverse every pair in the model lookup        
		readonly bool reversed;
		readonly int[] seqCounters = new int[NUMBER_OF_SEQ_CAT];
		int totalChar;

		int totalSeqs;

		public override ProbingState HandleData(byte[] buf, int offset, int len)
		{
			int max = offset + len;

			for (int i = offset; i < max; i++)
			{
				byte order = this.model.GetOrder(buf[i]);

				if (order < SYMBOL_CAT_ORDER) this.totalChar++;

				if (order < SAMPLE_SIZE)
				{
					this.freqChar++;

					if (this.lastOrder < SAMPLE_SIZE)
					{
						this.totalSeqs++;
						if (!this.reversed)
							++(this.seqCounters[this.model.GetPrecedence(this.lastOrder * SAMPLE_SIZE + order)]);
						else // reverse the order of the letters in the lookup
							++(this.seqCounters[this.model.GetPrecedence(order * SAMPLE_SIZE + this.lastOrder)]);
					}
				}
				this.lastOrder = order;
			}

			if (this.state == ProbingState.Detecting)
			{
				if (this.totalSeqs > SB_ENOUGH_REL_THRESHOLD)
				{
					float cf = this.GetConfidence();
					if (cf > POSITIVE_SHORTCUT_THRESHOLD)
						this.state = ProbingState.FoundIt;
					else if (cf < NEGATIVE_SHORTCUT_THRESHOLD) this.state = ProbingState.NotMe;
				}
			}
			return this.state;
		}

		public override void DumpStatus()
		{
			if (CharsetDetector.NeedConsoleLog)
			{
				Console.WriteLine("  SBCS: {0} [{1}]", this.GetConfidence(), this.GetCharsetName());
			}
		}

		public override float GetConfidence()
		{
			/*
			NEGATIVE_APPROACH
			if (totalSeqs > 0) {
			    if (totalSeqs > seqCounters[NEGATIVE_CAT] * 10)
			        return (totalSeqs - seqCounters[NEGATIVE_CAT] * 10)/totalSeqs * freqChar / mTotalChar;
			}
			return 0.01f;
			*/
			// POSITIVE_APPROACH

			if (this.totalSeqs > 0)
			{
				float r = 1.0f * this.seqCounters[POSITIVE_CAT] / this.totalSeqs / this.model.TypicalPositiveRatio;
				r = r * this.freqChar / this.totalChar;
				if (r >= 1.0f)
					r = 0.99f;
				return r;
			}
			return 0.01f;
		}

		public override void Reset()
		{
			this.state = ProbingState.Detecting;
			this.lastOrder = 255;
			for (int i = 0; i < NUMBER_OF_SEQ_CAT; i++) this.seqCounters[i] = 0;
			this.totalSeqs = 0;
			this.totalChar = 0;
			this.freqChar = 0;
		}

		public override string GetCharsetName()
		{
			return (this.nameProber == null)
				? this.model.CharsetName
				: this.nameProber.GetCharsetName();
		}
	}
}