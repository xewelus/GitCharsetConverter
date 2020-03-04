using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Ude;

namespace Converter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			this.InitializeComponent();

			this.analyzer.UnknownFinded += this.analyzer_UnknownFinded;
			this.analyzer.Win1251Finded += this.analyzer_Win1251Finded;
			this.analyzer.Error += this.analyzer_Error;
		}

		private readonly DirectoryAnalyzer analyzer = new DirectoryAnalyzer();

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				this.button1.Enabled = false;

				string dir = @"D:\Projects\AbbRetail2\RFCN-9349";

				this.lvFiles.Items.Clear();

				this.analyzer.ProcessDir(dir, null);
			}
			finally 
			{
				this.lvFiles.Columns[0].Width = -2;
				this.lvFiles.Columns[1].Width = -2;
				this.lvFiles.Columns[2].Width = -2;

				this.button1.Enabled = true;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string file = @"D:\projects\abbretail2\rfcn-9349\abr\common\abr.entities\03 client\NATURAL_PERSON.cs";
			this.analyzer.ProcessFile(file, null);
		}

		private void analyzer_UnknownFinded(string file, CharsetDetector detector)
		{
			ListViewItem listItem = this.lvFiles.Items.Add(file);
			listItem.SubItems.Add(detector.Charset);
			listItem.SubItems.Add(detector.Confidence.ToString());
			listItem.ForeColor = Color.Red;

			Application.DoEvents();
		}

		private void analyzer_Win1251Finded(string file, CharsetDetector detector)
		{
			return;

			ListViewItem listItem = this.lvFiles.Items.Add(file);
			listItem.SubItems.Add(detector.Charset);
			listItem.SubItems.Add(detector.Confidence.ToString());

			Application.DoEvents();
		}

		private void analyzer_Error(string file, Exception exception, CharsetDetector detector)
		{
			ListViewItem listItem = this.lvFiles.Items.Add(file);
			listItem.SubItems.Add("");
			listItem.SubItems.Add(exception.ToString());
		}

		private void lvFiles_ItemActivate(object sender, EventArgs e)
		{
			if (this.lvFiles.SelectedItems.Count == 0) return;

			string file = this.lvFiles.SelectedItems[0].Text;
			file = file.Replace("/", "\"");
			file = string.Format("\"{0}\"", file);
			Process.Start(file);
		}
	}
}
