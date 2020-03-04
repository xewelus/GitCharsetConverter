using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converter
{
	public static class StringUtils
	{
		public static string DecodeOctateString(string s)
		{
			StringBuilder sb = new StringBuilder();
			string code = null;
			List<byte> bytes = null;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				bool needRepeat = true;
				while (needRepeat)
				{
					needRepeat = false;
					bool isLast = i == s.Length - 1;
					bool needAppend = isLast;
					if (code == null)
					{
						if (c == '\\')
						{
							code = "";
							if (bytes == null)
							{
								bytes = new List<byte>();
							}
						}
						else
						{
							sb.Append(c);
						}
					}
					else
					{
						bool isDigit = char.IsDigit(c);
						if (isDigit)
						{
							code += c;
						}

						if (!isDigit || isLast)
						{
							int val = Convert.ToInt32(code, 8);
							bytes.Add((byte)val);

							if (c != '\\')
							{
								needAppend = true;
							}

							code = null;

							if (!isDigit)
							{
								needRepeat = true;
							}
						}

						if (needAppend && bytes.Count > 0)
						{
							byte[] array = bytes.ToArray();
							string str = Encoding.UTF8.GetString(array);
							sb.Append(str);
							bytes = null;
						}
					}
				}
			}
			return sb.ToString();
		}
	}
}
