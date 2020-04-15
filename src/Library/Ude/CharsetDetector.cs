using System.IO;
using Ude.Core;

namespace Ude
{
	/// <summary>
	/// Default implementation of charset detection interface. 
	/// The detector can be fed by a System.IO.Stream:
	/// <example>
	/// <code>
	/// using (FileStream fs = File.OpenRead(filename)) {
	///    CharsetDetector cdet = new CharsetDetector();
	///    cdet.Feed(fs);
	///    cdet.DataEnd();
	///    Console.WriteLine("{0}, {1}", cdet.Charset, cdet.Confidence);
	/// </code>
	/// </example>
	/// 
	///  or by a byte a array:
	/// 
	/// <example>
	/// <code>
	/// byte[] buff = new byte[1024];
	/// int read;
	/// while ((read = stream.Read(buff, 0, buff.Length)) > 0 && !done)
	///     Feed(buff, 0, read);
	/// cdet.DataEnd();
	/// Console.WriteLine("{0}, {1}", cdet.Charset, cdet.Confidence);
	/// </code>
	/// </example> 
	/// </summary>                
	public class CharsetDetector : UniversalDetector, ICharsetDetector
	{
		public CharsetDetector() : base(FILTER_ALL)
		{
		}

		public static bool NeedConsoleLog = false;

		public string Charset
		{
			get;
			private set;
		}

		public float Confidence
		{
			get;
			private set;
		}

		public bool IsDone()
		{
			return this.done;
		}

		protected override void Report(string charset, float confidence)
		{
			this.Charset = charset;
			this.Confidence = confidence;
		}

		#region ICharsetDetector Members

		public void Feed(Stream stream)
		{
			byte[] buff = new byte[1024];
			int read;
			while ((read = stream.Read(buff, 0, buff.Length)) > 0 && !this.done)
			{
				this.Feed(buff, 0, read);
			}
		}

		public override void Reset()
		{
			this.Charset = null;
			this.Confidence = 0.0f;
			base.Reset();
		}

		#endregion
	}
}