using System;
using System.Collections.Generic;
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

			this.WriteLine(string.Format("> {0}: {1} {2}", workDir ?? Directory.GetCurrentDirectory(), filename, argsString), COLOR_COMMAND);

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

		private OutputHandler outputHandler;
		private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				if (this.batchLogObj == null)
				{
					LogConsole(e.Data);
					this.WriteLine(e.Data);
				}
				else
				{
					this.batchLogObj.Add(e.Data);
				}

				if (this.outputHandler != null)
				{
					this.outputHandler.Handle(e.Data);
				}
			}
		}

		private BatchLogObj batchLogObj;
		private BatchLogObj BatchLog()
		{
			if (this.batchLogObj != null)
			{
				throw new Exception("batchLogObj");
			}

			this.batchLogObj = new BatchLogObj(this);
			return this.batchLogObj;
		}

		private class BatchLogObj : IDisposable
		{
			private readonly ConsoleForm form;
			private readonly StringBuilder stringBuilder = new StringBuilder();

			public BatchLogObj(ConsoleForm form)
			{
				this.form = form;
			}

			public void Add(string data)
			{
				this.stringBuilder.AppendLine(data);
			}

			public void Dispose()
			{
				this.form.batchLogObj = null;
				string str = this.stringBuilder.ToString();
				LogConsole(str);
				this.form.WriteLine(str);
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
		private void InputLine(string text, bool wait = true)
		{
			while (wait && this.lastInputTime != null && DateTime.Now.Subtract(this.lastInputTime.Value).TotalMilliseconds < 100)
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

		private void btnGit3_Click(object sender, EventArgs e)
		{
			string range;
			if (this.tbFrom.TextLength == 0 && this.tbTo.TextLength == 0)
			{
				range = "--all";
			}
			else if (this.tbFrom.TextLength == 0)
			{
				range = this.tbTo.Text;
			}
			else
			{
				range = $"{this.tbFrom.Text}..{this.tbTo.Text}";
			}

			this.WriteLine(((Control)sender).Text, COLOR_COMMAND);

			this.InputLine("set FILTER_BRANCH_SQUELCH_WARNING=1");

			string dir = DIR_testenc2;
			this.InputLine(Directory.GetDirectoryRoot(dir).Replace("\\", ""));
			this.InputLine("cd " + dir);

			string exePath = Assembly.GetExecutingAssembly().Location;
			exePath = exePath.Replace("\\", "/");
			string args = string.Format(@"filter-branch --tree-filter ""{0} -git"" -f --tag-name-filter cat -- {1}", exePath, range);
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

		private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
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

		private void btnCR_Click(object sender, EventArgs e)
		{
			this.WriteLine(((Control)sender).Text, COLOR_COMMAND);

			string projDir = "r";
			this.FilterBranch(projDir);
		}

		private void FilterBranch(string projDir)
		{
			this.InputLine("set FILTER_BRANCH_SQUELCH_WARNING=1");

			this.InputCd(@"C:\r");

			string exePath = Assembly.GetExecutingAssembly().Location;
			exePath = exePath.Replace("\\", "/");
			string args = string.Format(@"filter-branch --tree-filter ""{0} -git {1}"" -f --tag-name-filter cat -- --all", exePath, projDir);

			this.InputLine("git " + args);
		}

		private void InputCd(string dir)
		{
			this.InputLine(Directory.GetDirectoryRoot(dir).Replace("\\", ""));
			this.InputLine("cd " + dir);
		}

		private class OutputHandler
		{
			public DateTime? LastTime;
			public void Handle(string text)
			{
				this.LastTime = DateTime.Now;
				this.InternalHandle(text);
			}

			protected virtual void InternalHandle(string text)
			{
			}
		}

		private void WaitOutput()
		{
			while (true)
			{
				if (this.outputHandler.LastTime == null || DateTime.Now.Subtract(this.outputHandler.LastTime.Value).TotalSeconds < 1)
				{
					Application.DoEvents();
					continue;
				}

				break;
			}
		}

		private class RewriteLinesAnalyzer : OutputHandler
		{
			protected override void InternalHandle(string text)
			{
			}
		}

		private class LinesAnalyzer : OutputHandler
		{
			public List<string> Lines = new List<string>();
			protected override void InternalHandle(string text)
			{
				this.Lines.Add(text);
			}
		}

		private void btnGetCRFiles_Click(object sender, EventArgs e)
		{
			this.WriteLine(((Control)sender).Text, COLOR_COMMAND);
			List<string> commits = this.GetCommits(@"C:\r");
			this.WriteCommitsFiles(commits);
		}

		private void WriteCommitsFiles(List<string> commits)
		{
			string project = "r";

			string projDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, project);
			if (!Directory.Exists(projDir))
			{
				Directory.CreateDirectory(projDir);
			}

			string commitsDir = Path.Combine(projDir, "commits");
			if (!Directory.Exists(commitsDir))
			{
				Directory.CreateDirectory(commitsDir);
			}

			foreach (string file in Directory.GetFiles(commitsDir))
			{
				File.Delete(file);
			}

			using (this.BatchLog())
			{
				foreach (string commit in commits)
				{
					if (!IsCommit(commit)) continue;

					this.WriteChangedFiles(commitsDir, commit);
				}
			}
		}

		private static bool IsCommit(string commit)
		{
			if (commit.Length != 40) return false;
			return true;
		}

		private List<string> GetCommits(string folder)
		{
			this.InputCd(folder);

			LinesAnalyzer linesAnalyzer = new LinesAnalyzer();
			this.outputHandler = linesAnalyzer;

			using (this.BatchLog())
			{
				this.InputLine("git rev-list --all");
				this.WaitOutput();
				this.outputHandler = null;
				return linesAnalyzer.Lines;
			}
		}

		private void WriteChangedFiles(string commitsDir, string commit)
		{
			//LinesAnalyzer linesAnalyzer = new LinesAnalyzer();
			//this.outputHandler = linesAnalyzer;

			//string cmd = string.Format("git diff-tree --no-commit-id --name-only -r {0}", commit); //"--output=<file>"

			string output = Path.Combine(commitsDir, commit + ".txt");
			string cmd = string.Format("git diff-tree --no-commit-id --name-only -r -m {0} --output=\"{1}\"", commit, output);
			this.InputLine(cmd, false);

			//this.WaitOutput();
			//return linesAnalyzer.Lines;
		}
	}
}
