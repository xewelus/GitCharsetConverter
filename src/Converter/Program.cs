using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ude;

namespace Converter
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				if (args[0] == "-git")
				{
					string folder = Directory.GetCurrentDirectory();

					//Session session = Session.Load();
					//DateTime? time = session.GetLastTime(folder);
					//ProcessFolder(folder, time);
					//session.SetLastTime(folder, DateTime.Now);

					ProcessFolder(folder, null);

					return;
				}
				MessageBox.Show(args[0]);
				return;
			}

			Application.ThreadException += OnApplicationOnThreadException;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//Application.Run(new MainForm());
			//Tests.ConsoleRedirect.Test();
			Application.Run(new ConsoleForm());
		}

		public static void ConvertFile(string file)
		{
			Encoding win1251 = Encoding.GetEncoding("windows-1251");
			string text = File.ReadAllText(file, win1251);
			File.WriteAllText(file, text, Encoding.UTF8);

			//MessageBox.Show(text, file);
		}

		public static void ProcessFolder(string folder, DateTime? dateMoreThan)
		{
			try
			{
				DirectoryAnalyzer analyzer = new DirectoryAnalyzer();
				analyzer.Error += AnalyzerOnError;
				analyzer.Win1251Finded += AnalyzerWin1251Finded;
				analyzer.ProcessDir(folder, dateMoreThan);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void AnalyzerWin1251Finded(string file, CharsetDetector detector)
		{
			Log(file);
			ConvertFile(file);
		}

		private static void AnalyzerOnError(string file, Exception exception, CharsetDetector detector)
		{
			Log(string.Format("Ошибка {0}:\r\n{1}\r\n", file, exception));
			MessageBox.Show(file + ":\r\n\r\n" + exception, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void Log(string message)
		{
			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			File.AppendAllText(logFile, string.Format("\r\n[{0:dd.MM.yyyy HH.mm:ss}] {1}", DateTime.Now, message), Encoding.UTF8);
		}

		private static void OnApplicationOnThreadException(object sender, ThreadExceptionEventArgs args)
		{
			MessageBox.Show(args.Exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
