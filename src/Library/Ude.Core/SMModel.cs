using System;

namespace Ude.Core
{
	/// <summary>
	/// State machine model
	/// </summary>
	public abstract class SMModel
	{
		public SMModel(BitPackage classTable, int classFactor,
			BitPackage stateTable, int[] charLenTable, String name)
		{
			this.classTable = classTable;
			this.classFactor = classFactor;
			this.stateTable = stateTable;
			this.charLenTable = charLenTable;
			this.name = name;
		}

		public const int START = 0;
		public const int ERROR = 1;
		public const int ITSME = 2;
		public int[] charLenTable;

		private readonly int classFactor;

		public BitPackage classTable;

		private readonly string name;
		public BitPackage stateTable;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public int ClassFactor
		{
			get
			{
				return this.classFactor;
			}
		}

		public int GetClass(byte b)
		{
			return this.classTable.Unpack(b);
		}
	}
}