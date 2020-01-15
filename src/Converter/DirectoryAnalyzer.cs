using System;
using System.IO;
using Ude;

namespace Converter
{
	public class DirectoryAnalyzer
	{
		private readonly CharsetDetector detector;

		public delegate void FileProcessDelegate(string file, CharsetDetector detector);
		public event FileProcessDelegate FileProcess;
		public event FileProcessDelegate Win1251Finded;
		public event FileProcessDelegate UnknownFinded;

		public delegate void OnErrorDelegate(string file, Exception exception, CharsetDetector detector);
		public event OnErrorDelegate Error;

		public DirectoryAnalyzer() : this(new CharsetDetector())
		{
		}

		public DirectoryAnalyzer(CharsetDetector detector)
		{
			this.detector = detector;
		}

		public void ProcessDir(string dir)
		{
			dir = dir.ToLower();

			string folder = Path.GetFileName(dir);
			if (folder == ".hg") return;
			if (folder == ".vs") return;
			if (dir.Contains(@"bin\debug")) return;
			if (dir.Contains(@"bin\release")) return;
			if (dir.Contains(@"obj\debug")) return;
			if (dir.Contains(@"obj\release")) return;
			if (dir.Contains(@"resharper.caches")) return;
			if (dir.Contains(@"abr\installers\content.update")) return;
			
			foreach (string file in Directory.GetFiles(dir))
			{
				this.ProcessFile(file);
			}
			foreach (string subdir in Directory.GetDirectories(dir))
			{
				this.ProcessDir(subdir);
			}
		}

		public void ProcessFile(string file)
		{
			string ext = Path.GetExtension(file);
			if (ext != null)
			{
				ext = ext.ToLower();
				if (ext == ".dll") return;
				if (ext == ".pdb") return;
				if (ext == ".exe") return;
				if (ext == ".png") return;
				if (ext == ".gif") return;
				if (ext == ".ico") return;
				if (ext == ".bmp") return;
				if (ext == ".doc") return;
				if (ext == ".xls") return;
				if (ext == ".rar") return;
				if (ext == ".msi") return;
				if (ext == ".dbf") return;
				if (ext == ".snk") return;
				if (ext == ".dat") return;
				if (ext == ".pfx") return;
				if (ext == ".nupkg") return;
			}

			try
			{
				using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					this.detector.Reset();
					this.detector.Feed(fs);
					this.detector.DataEnd();

					if (this.FileProcess != null)
					{
						this.FileProcess.Invoke(file, this.detector);
					}

					//if (Math.Abs(this.detector.Confidence - 1f) < 0.1f)
					{
						if (this.detector.Charset == "UTF-8") return;
						if (this.detector.Charset == "UTF-16LE") return;
						if (this.detector.Charset == "ASCII") return;
						if (this.detector.Charset == "windows-1251" 
							//|| this.detector.Charset == "x-mac-cyrillic"
						    //|| this.detector.Charset == "windows-1252"
						    )
						{
							if (this.Win1251Finded != null)
							{
								this.Win1251Finded.Invoke(file, this.detector);
							}
							return;
						}
					}

					if (this.UnknownFinded != null)
					{
						this.UnknownFinded.Invoke(file, this.detector);
					}
				}
			}
			catch (Exception ex)
			{
				if (this.Error != null)
				{
					this.Error.Invoke(file, ex, this.detector);
				}
			}
		}
	}
}
