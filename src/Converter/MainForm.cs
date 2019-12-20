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
		}

		private void ProcessDir(string dir)
		{
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
			using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Console.WriteLine("Analysing {0}", file);
				this.detector.Feed(fs);
				this.detector.DataEnd();

				ListViewItem listItem = this.lvFiles.Items.Add(file);
				listItem.SubItems.Add(this.detector.Charset);
				listItem.SubItems.Add(this.detector.Confidence.ToString());
				this.detector.Reset();
			}
		}
	}
}
