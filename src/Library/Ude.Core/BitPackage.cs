namespace Ude.Core
{
	public class BitPackage
	{
		public BitPackage(int indexShift, int shiftMask,
			int bitShift, int unitMask, int[] data)
		{
			this.indexShift = indexShift;
			this.shiftMask = shiftMask;
			this.bitShift = bitShift;
			this.unitMask = unitMask;
			this.data = data;
		}

		public static int INDEX_SHIFT_4BITS = 3;
		public static int INDEX_SHIFT_8BITS = 2;
		public static int INDEX_SHIFT_16BITS = 1;

		public static int SHIFT_MASK_4BITS = 7;
		public static int SHIFT_MASK_8BITS = 3;
		public static int SHIFT_MASK_16BITS = 1;

		public static int BIT_SHIFT_4BITS = 2;
		public static int BIT_SHIFT_8BITS = 3;
		public static int BIT_SHIFT_16BITS = 4;

		public static int UNIT_MASK_4BITS = 0x0000000F;
		public static int UNIT_MASK_8BITS = 0x000000FF;
		public static int UNIT_MASK_16BITS = 0x0000FFFF;
		private readonly int bitShift;
		private readonly int[] data;

		private readonly int indexShift;
		private readonly int shiftMask;
		private readonly int unitMask;

		public static int Pack16bits(int a, int b)
		{
			return ((b << 16) | a);
		}

		public static int Pack8bits(int a, int b, int c, int d)
		{
			return Pack16bits((b << 8) | a, (d << 8) | c);
		}

		public static int Pack4bits(int a, int b, int c, int d,
			int e, int f, int g, int h)
		{
			return Pack8bits((b << 4) | a, (d << 4) | c,
			                 (f << 4) | e, (h << 4) | g);
		}

		public int Unpack(int i)
		{
			return (this.data[i >> this.indexShift] >>
			        ((i & this.shiftMask) << this.bitShift)) & this.unitMask;
		}
	}
}