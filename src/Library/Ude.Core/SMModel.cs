namespace Ude.Core
{
	/// <summary>
	/// State machine model
	/// </summary>
	public abstract class SMModel
	{
		protected SMModel(BitPackage classTable, int classFactor,
			BitPackage stateTable, int[] charLenTable, string name)
		{
			this.classTable = classTable;
			this.ClassFactor = classFactor;
			this.stateTable = stateTable;
			this.charLenTable = charLenTable;
			this.Name = name;
		}

		public const int START = 0;
		public const int ERROR = 1;
		public const int ITSME = 2;
		public readonly int[] charLenTable;
		private readonly BitPackage classTable;

		public readonly BitPackage stateTable;

		public string Name
		{
			get;
		}

		public int ClassFactor { get; }

		public int GetClass(byte b)
		{
			return this.classTable.Unpack(b);
		}
	}
}