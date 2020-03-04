using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ude;

namespace Converter
{
	public static class UtilApp
	{
		public static void Process(string[] args)
		{
			if (args.Length < 2)
			{
				MessageBox.Show("Не заданы необходимые аргументы.");
				return;
			}
			string projDir = args[1];
			string folder = Directory.GetCurrentDirectory();

			//Session session = Session.Load();
			//DateTime? time = session.GetLastTime(folder);
			//ProcessFolder(folder, time);
			//session.SetLastTime(folder, DateTime.Now);

			//MessageBox.Show("Start process folder.");

			List<string> files = GetFiles(folder, projDir);
			ProcessFolder(folder, null, files);
		}


		private static List<string> GetFiles(string folder, string projDir)
		{
			string parentSha = GetParentSha(folder);
			if (parentSha == null)
			{
				return null;
			}

			//MessageBox.Show("parent: " + parentSha);

			List<string> commits = GetCommits(folder, parentSha);
			if (commits == null)
			{
				return null;
			}

			List<string> result = null;
			foreach (string commit in commits)
			{
				List<string> files = GetCommitFiles(commit, projDir);
				if (files != null)
				{
					//MessageBox.Show("files: " + files.Count);

					if (result == null)
					{
						result = files;
					}
					else
					{
						result.AddRange(files);
					}
				}
			}
			return result;
		}

		private static List<string> GetCommits(string folder, string parentSha)
		{
			string revsPath = Path.GetFullPath(Path.Combine(folder, "../revs"));
			using (StreamReader sr = new StreamReader(revsPath, Encoding.UTF8))
			{
				List<string> commits = new List<string>();
				while (true)
				{
					string line = sr.ReadLine();
					if (string.IsNullOrEmpty(line))
					{
						break;
					}

					string[] parts = line.Split(' ');
					for (int i = 1; i < parts.Length; i++)
					{
						if (parts[i] == parentSha)
						{
							commits.Add(parts[0]);
							break;
						}
					}
				}
				return commits;
			}
		}

		private static string GetParentSha(string folder)
		{
			string revsPath = Path.GetFullPath(Path.Combine(folder, "../commit"));
			using (StreamReader sr = new StreamReader(revsPath, Encoding.UTF8))
			{
				while (true)
				{
					string line = sr.ReadLine();

					if (string.IsNullOrEmpty(line))
					{
						return null;
					}

					if (line.StartsWith("parent "))
					{
						return line.Substring("parent ".Length);
					}
				}
			}
		}

		private static List<string> GetCommitFiles(string commit, string projDir)
		{
			string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, projDir);
			dir = Path.Combine(dir, "commits");
			string file = Path.Combine(dir, commit + ".txt");
			if (!File.Exists(file))
			{
				return null;
			}
			string[] lines = File.ReadAllLines(file, Encoding.UTF8);
			List<string> list = new List<string>();
			foreach (string line in lines)
			{
				if (!string.IsNullOrEmpty(line))
				{
					list.Add(line.ToLower().Replace("/", "\\"));
				}
			}
			return list;
		}

		public static void ConvertFile(string file)
		{
			Encoding win1251 = Encoding.GetEncoding("windows-1251");
			string text = File.ReadAllText(file, win1251);
			File.WriteAllText(file, text, Encoding.UTF8);

			//MessageBox.Show(text, file);
		}

		public static void ProcessFolder(string folder, DateTime? dateMoreThan, List<string> files)
		{
			try
			{
				DirectoryAnalyzer analyzer = new DirectoryAnalyzer();
				analyzer.Error += AnalyzerOnError;
				analyzer.Win1251Finded += AnalyzerWin1251Finded;
				analyzer.ProcessDir(folder, dateMoreThan, folder, files);
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
