using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Converter
{
	public partial class ConsoleForm : Form
	{
		public ConsoleForm()
		{
			this.InitializeComponent();
		}

		private Process process;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.RunProcess();
		}

		private void RunProcess()
		{
			this.process = new Process();
			this.process.StartInfo.FileName = "cmd";
			this.process.StartInfo.CreateNoWindow = true;
			this.process.StartInfo.UseShellExecute = false;
			this.process.StartInfo.RedirectStandardInput = true;
			this.process.StartInfo.RedirectStandardOutput = true;
			this.process.StartInfo.RedirectStandardError = true;
			this.process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
			this.process.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(866);
			this.process.ErrorDataReceived += this.ProcessOnErrorDataReceived;
			this.process.OutputDataReceived += this.ProcessOnOutputDataReceived;
			this.process.Start();
			this.process.BeginOutputReadLine();
			this.process.BeginErrorReadLine();
		}

		private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				LogConsole("ERROR: " + e.Data, "console.log");
				this.WriteLine(e.Data, Color.Red);
			}
		}

		private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				LogConsole(e.Data, "console.log");
				this.WriteLine(e.Data);
			}
		}

		private void WriteLine(string data, Color? color = null)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Action<string, Color?>(this.WriteLine), data, color);
				return;
			}

			if (color == null)
			{
				color = Color.White;
			}

			this.tbText.SelectionColor = color.Value;

			data = data ?? string.Empty;
			this.tbText.AppendText(data);
			this.tbText.AppendText(Environment.NewLine);
		}

		private static void LogConsole(string text, string logfile)
		{
			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logfile);
			File.AppendAllText(logFile, "[" + DateTime.Now + "] ", Encoding.UTF8);
			File.AppendAllText(logFile, text + "\r\n", Encoding.UTF8);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.InputLine("git2 --version");
		}

		private void InputLine(string text)
		{
			text = "> " + text;
			this.process.StandardInput.WriteLine(text);
			this.WriteLine(text);
		}
	}
}
