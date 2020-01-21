using System;
using System.Diagnostics;
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
			this.RunProcess();
		}

		private Process process;

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
			this.process.ErrorDataReceived += this.ProcessOnErrorDataReceived;
			this.process.OutputDataReceived += this.ProcessOnOutputDataReceived;
			this.process.Start();
			this.process.BeginOutputReadLine();
		}

		private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			LogConsole(e.Data, "console_errors.log");
			this.WriteLine("Ошибка:");
			this.WriteLine(e.Data);
		}

		private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			LogConsole(e.Data, "console.log");
			this.WriteLine(e.Data);
		}

		private void WriteLine(string data)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action<string>(this.WriteLine), data);
				return;
			}

			data = data ?? string.Empty;
			this.tbText.AppendText(data);
			this.tbText.AppendText(Environment.NewLine);
		}

		private static void LogConsole(string text, string logfile)
		{
			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logfile);
			File.AppendAllText(logFile, "[" + DateTime.Now + "]\r\n", Encoding.UTF8);
			File.AppendAllText(logFile, text + "\r\n", Encoding.UTF8);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.process.StandardInput.WriteLine("git --version");
		}
	}
}
