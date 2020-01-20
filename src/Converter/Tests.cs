using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ude;

namespace Converter
{
	public static class Tests
	{
		public static void a()
		{

		}


		private static void TestConvert()
		{
			string file = @"D:\git\testenc2\windows-1251.txt";
			ConvertFile(file);
		}

		private static void ConvertFile(string file)
		{
			Encoding win1251 = Encoding.GetEncoding("windows-1251");
			string text = File.ReadAllText(file, win1251);
			File.WriteAllText(file, text, Encoding.UTF8);

			MessageBox.Show(text, file);
		}

		private static void ProcessFolder()
		{
			try
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

				DirectoryAnalyzer analyzer = new DirectoryAnalyzer();
				analyzer.Error += AnalyzerOnError;
				analyzer.Win1251Finded += AnalyzerWin1251Finded;
				analyzer.ProcessDir(folder);
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
	}
}
