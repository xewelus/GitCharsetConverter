namespace Ude.Core
{
	/// <summary>
	/// Parallel state machine for the Coding Scheme Method
	/// </summary>
	public class CodingStateMachine
	{
		public CodingStateMachine(SMModel model)
		{
			this.currentState = SMModel.START;
			this.model = model;
		}

		private int currentState;
		private readonly SMModel model;

		public int CurrentCharLen { get; private set; }

		public string ModelName
		{
			get
			{
				return this.model.Name;
			}
		}

		public int NextState(byte b)
		{
			// for each byte we get its class, if it is first byte, 
			// we also get byte length
			int byteCls = this.model.GetClass(b);
			if (this.currentState == SMModel.START)
			{
				this.CurrentCharLen = this.model.charLenTable[byteCls];
			}

			// from byte's class and stateTable, we get its next state            
			this.currentState = this.model.stateTable.Unpack(this.currentState * this.model.ClassFactor + byteCls);
			return this.currentState;
		}

		public void Reset()
		{
			this.currentState = SMModel.START;
		}
	}
}