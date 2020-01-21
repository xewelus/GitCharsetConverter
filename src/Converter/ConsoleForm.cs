using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
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
			//this.RunProcess(null, "cmd");
			//this.RunProcess(@"D:\git\testenc2");
		}

		private void RunProcess(string workDir, string filename, params string[] args)
		{
			string argsString = args.Length == 0 ? null : string.Join(" ", args);

			this.WriteLine(string.Format("> {0}: {1} {2}", workDir ?? Directory.GetCurrentDirectory(), filename, argsString));

			this.process = new Process();
			this.process.StartInfo.FileName = filename;
			if (workDir != null)
			{
				this.process.StartInfo.WorkingDirectory = workDir;
			}
			if (argsString != null)
			{
				this.process.StartInfo.Arguments = argsString;
			}
			this.process.StartInfo.CreateNoWindow = true;
			this.process.StartInfo.UseShellExecute = false;
			this.process.StartInfo.RedirectStandardInput = true;
			this.process.StartInfo.RedirectStandardOutput = true;
			this.process.StartInfo.RedirectStandardError = true;
			this.process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
			this.process.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(866);
			this.process.StartInfo.Verb = "runas";
			//this.process.StartInfo.EnvironmentVariables.Add("FILTER_BRANCH_SQUELCH_WARNING", "1");
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

			if (this.cbScroll.Checked)
			{
				this.tbText.ScrollToCaret();
			}
		}

		private static void LogConsole(string text, string logfile)
		{
			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logfile);
			File.AppendAllText(logFile, "[" + DateTime.Now + "] ", Encoding.UTF8);
			File.AppendAllText(logFile, text + "\r\n", Encoding.UTF8);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string exePath = Assembly.GetExecutingAssembly().Location;
			exePath = exePath.Replace("\\", "/");
			string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f -- --all", exePath);
			this.RunProcess(@"D:\git\testenc2", "git", args);
		}

		private DateTime? lastInputTime;
		private void InputLine(string text)
		{
			while (this.lastInputTime != null && DateTime.Now.Subtract(this.lastInputTime.Value).TotalMilliseconds < 100)
			{
				Application.DoEvents();
			}


			this.process.StandardInput.WriteLine(text);
			this.WriteLine("> " + text);

			this.lastInputTime = DateTime.Now;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.RunProcess(@"D:\git\testenc2", "cmd");

		}

		private void button3_Click(object sender, EventArgs e)
		{
			this.InputLine("set");
		}

		private void button4_Click(object sender, EventArgs e)
		{
			this.InputLine("set FILTER_BRANCH_SQUELCH_WARNING=1");

			string exePath = Assembly.GetExecutingAssembly().Location;
			exePath = exePath.Replace("\\", "/");
			string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f -- --all", exePath);
			this.InputLine("git " + args);
		}
	}
}
