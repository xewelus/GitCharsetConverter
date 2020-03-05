using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibGit2Sharp;
using Ude;

namespace Converter
{
	public static class Tests
	{
		public static class ConsoleRedirect
		{
			public static void Test()
			{
				Process process = new Process();
				process.StartInfo.FileName = "cmd";
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.ErrorDataReceived += ProcessOnErrorDataReceived;
				process.OutputDataReceived += ProcessOnOutputDataReceived;
				process.Start();
				process.BeginOutputReadLine();

				process.StandardInput.WriteLine("git");

				process.WaitForExit();
			}

			private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
			{
				LogConsole(e.Data, "console_errors.log");
			}

			private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
			{
				LogConsole(e.Data, "console.log");
			}

			private static void LogConsole(string text, string logfile)
			{
				string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logfile);
				File.AppendAllText(logFile, "[" + DateTime.Now + "]\r\n", Encoding.UTF8);
				File.AppendAllText(logFile, text + "\r\n", Encoding.UTF8);
			}
		}

		private static void TestConvert()
		{
			string file = @"D:\git\testenc2\windows-1251.txt";
			UtilApp.ConvertFile(file);
		}

		public static void ProcessFolder()
		{
			string folder = ConfigurationManager.AppSettings["RepositoryFolder"];
			if (string.IsNullOrEmpty(folder))
			{
				MessageBox.Show("Не задан параметр RepositoryFolder.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (!Directory.Exists(folder))
			{
				MessageBox.Show("Не найдена папка " + folder, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			folder = Path.Combine(folder, @".git-rewrite\t");
			if (!Directory.Exists(folder)) return;

			UtilApp.ProcessFolder(folder, null);
		}

		private static void TestLibGit()
		{
			string result;
			using (var repo = new Repository("C:/r"))
			{
				List<Commit> CommitList = new List<Commit>();
				//foreach (LogEntry entry in repo.Commits.QueryBy("relative/path/to/your/file").ToList())
				foreach (var entry in repo.Commits)
				{
					CommitList.Add(entry);
				}
				CommitList.Add(null); // Added to show correct initial add

				int ChangeDesired = 0; // Change difference desired
				var repoDifferences = repo.Diff.Compare<Patch>((Equals(CommitList[ChangeDesired + 1], null)) ? null : CommitList[ChangeDesired + 1].Tree, (Equals(CommitList[ChangeDesired], null)) ? null : CommitList[ChangeDesired].Tree);
				PatchEntryChanges file = null;
				try { file = repoDifferences.First(e => e.Path == "relative/path/to/your/file"); }
				catch { } // If the file has been renamed in the past- this search will fail
				if (!Equals(file, null))
				{
					result = file.Patch;
				}
			}
		}
	}
}
