using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
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
			this.RunProcess(null, "cmd");
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
				LogConsole("ERROR: " + e.Data);
				this.WriteLine(e.Data, Color.Red);
			}
		}

		private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				LogConsole(e.Data);
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

			LogConsole(data);

			if (this.cbScroll.Checked)
			{
				this.tbText.ScrollToCaret();
			}
		}

		private static void LogConsole(string text, string logfile = "console.log")
		{
			lock ("LogConsole")
			{
				string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logfile);
				File.AppendAllText(logFile, "[" + DateTime.Now + "] ", Encoding.UTF8);
				File.AppendAllText(logFile, text + "\r\n", Encoding.UTF8);
			}
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
			this.WriteLine("> " + text, Color.YellowGreen);

			this.lastInputTime = DateTime.Now;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			this.WriteLine(((Control)sender).Text, COLOR_COMMAND);

			this.InputLine("set FILTER_BRANCH_SQUELCH_WARNING=1");

			string dir = DIR_testenc2;
			this.InputLine(Directory.GetDirectoryRoot(dir).Replace("\\", ""));
			this.InputLine("cd " + dir);

			string exePath = Assembly.GetExecutingAssembly().Location;
			exePath = exePath.Replace("\\", "/");
			string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f --tag-name-filter cat -- --all", exePath);
			//string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f --tag-name-filter cat -- tag2", exePath);
			//string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f --tag-name-filter cat -- master", exePath);
			this.InputLine("git " + args);
		}

		private const string DIR_testenc = @"D:\git\testenc";
		private const string DIR_testenc2 = @"D:\git\testenc2";
		private static readonly Color COLOR_COMMAND = Color.DeepSkyBlue;
		private void button5_Click(object sender, EventArgs e)
		{
			ClearDir(DIR_testenc2);
			CopyFilesRecursively(new DirectoryInfo(DIR_testenc), new DirectoryInfo(DIR_testenc2));
			this.WriteLine("Замена папки завершена.", COLOR_COMMAND);
			//MessageBox.Show("Замена папки завершена.");
		}

		private static void ClearDir(string directory)
		{
			DirectoryInfo info = new DirectoryInfo(directory);
			ClearDir(info);
		}

		private static void ClearDir(DirectoryInfo info)
		{
			foreach (FileInfo file in info.GetFiles())
			{
				File.SetAttributes(file.FullName, FileAttributes.Normal);
				file.SetAccessControl(new FileSecurity());
				file.Delete();
			}
			foreach (DirectoryInfo dir in info.GetDirectories())
			{
				ClearDir(dir);
				dir.Delete();
			}
		}

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
		{
			if (!target.Exists)
			{
				target.Create();
			}
			foreach (DirectoryInfo dir in source.GetDirectories())
			{
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			}
			foreach (FileInfo file in source.GetFiles())
			{ 
				file.CopyTo(Path.Combine(target.FullName, file.Name));
			}
		}

		private void cbWordWrap_CheckedChanged(object sender, EventArgs e)
		{
			this.tbText.WordWrap = this.cbWordWrap.Checked;
		}
	}
}
