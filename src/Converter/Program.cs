using System;
using System.Configuration;
using System.IO;
using System.Text;
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
		static void Main()
		{
			Application.ThreadException += (sender, args) => MessageBox.Show(args.Exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

			//Process();
			TestConvert();


			//return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private static void TestConvert()
		{
			string file = @"D:\git\testenc2\windows-1251.txt";
			Encoding win1251 = Encoding.GetEncoding("windows-1251");
			string text = File.ReadAllText(file, win1251);
			File.WriteAllText(file, text, Encoding.UTF8);
		}

		private static void Process()
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

				DirectoryAnalyzer analyzer = new DirectoryAnalyzer();
				analyzer.Error += AnalyzerOnError;
				analyzer.Win1251Finded += AnalyzerOnAsciiFinded;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void AnalyzerOnAsciiFinded(string file, CharsetDetector detector)
		{
			throw new NotImplementedException();
		}

		private static void AnalyzerOnError(string file, Exception exception, CharsetDetector detector)
		{
			MessageBox.Show(file + ":\r\n\r\n" + exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
