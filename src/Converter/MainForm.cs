using System;
using System.IO;
using System.Windows.Forms;
using Ude;

namespace Converter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			this.InitializeComponent();
		}

		private CharsetDetector detector = new CharsetDetector();
		private void button1_Click(object sender, EventArgs e)
		{
			string dir = @"D:\Projects\AbbRetail2\RFCN-9349";

			this.lvFiles.Items.Clear();

			ProcessDir(dir);

			this.lvFiles.Columns[0].Width = -2;
			this.lvFiles.Columns[1].Width = -2;
			this.lvFiles.Columns[2].Width = -2;
		}

		private void ProcessDir(string dir)
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
			
			foreach (string file in Directory.GetFiles(dir))
			{
				ProcessFile(file);
			}
			foreach (string subdir in Directory.GetDirectories(dir))
			{
				ProcessDir(subdir);
			}
		}

		private void ProcessFile(string file)
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
			}

			try
			{
				using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					this.detector.Reset();
					this.detector.Feed(fs);
					this.detector.DataEnd();

					if (Math.Abs(this.detector.Confidence - 1f) < 0.01f)
					{
						if (this.detector.Charset == "UTF-8") return;
						if (this.detector.Charset == "ASCII") return;
					}

					ListViewItem listItem = this.lvFiles.Items.Add(file);
					listItem.SubItems.Add(this.detector.Charset);
					listItem.SubItems.Add(this.detector.Confidence.ToString());

					Application.DoEvents();
				}
			}
			catch (Exception ex)
			{
				ListViewItem listItem = this.lvFiles.Items.Add(file);
				listItem.SubItems.Add("");
				listItem.SubItems.Add(ex.ToString());
			}
		}
	}
}
