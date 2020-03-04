using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
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
	}
}
