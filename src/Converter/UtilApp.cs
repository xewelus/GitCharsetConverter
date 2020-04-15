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
		public static bool LOG_ENABLED = true;
		public static bool ONLY_COMMIT_FILES = false;
		private static string logPrefix = new string(' ', 7);
		public static void Process(string[] args)
		{
			try
			{
				if (args.Length < 2)
				{
					throw new Exception("Не заданы необходимые аргументы.");
				}
				string projDir = args[1];
				string folder = Directory.GetCurrentDirectory();

				if (LOG_ENABLED)
				{
					Log(string.Format("Project alias: {0}, folder: {1}", projDir, folder), false, true);
				}

				List<string> files = ONLY_COMMIT_FILES ? GetFiles(folder, projDir) : null;
				ProcessFolder(folder, files);
			}
			catch (Exception ex)
			{
				Log(ex.ToString(), true);
				//MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static List<string> GetFiles(string folder, string projDir)
		{
			string parentSha = GetParentSha(folder);
			if (parentSha == null)
			{
				return null;
			}

			logPrefix = parentSha.Substring(0, 7);
			if (LOG_ENABLED)
			{
				Log(string.Format("Parent: {0}", parentSha));
			}

			List<string> commits = GetCommits(folder, parentSha);
			if (commits == null)
			{
				if (LOG_ENABLED)
				{
					Log("No commits found.");
				}
				return null;
			}

			if (LOG_ENABLED)
			{
				Log(string.Format("Commits found: {0}", string.Join(", ", commits.ToArray())));
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
						foreach (string file in files)
						{
							if (!result.Contains(file))
							{
								result.Add(file);
							}
						}
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
					string path = line;
					if (path.StartsWith("\""))
					{
						path = StringUtils.DecodeOctateString(line).Trim('\"');
					}
					list.Add(path.ToLower().Replace("/", "\\"));
				}
			}
			return list;
		}

		public static void ConvertFile(string file)
		{
			Encoding win1251 = Encoding.GetEncoding("windows-1251");
			string text = File.ReadAllText(file, win1251);
			File.WriteAllText(file, text, Encoding.UTF8);
		}

		public static void ProcessFolder(string folder, List<string> files)
		{
			try
			{
				DirectoryAnalyzer analyzer = new DirectoryAnalyzer();
				analyzer.Error += AnalyzerOnError;
				analyzer.Win1251Finded += AnalyzerWin1251Finded;

				if (files == null)
				{
					analyzer.ProcessDir(folder);

					if (LOG_ENABLED)
					{
						Log("Process all files mode.");
					}
				}
				else
				{
					if (LOG_ENABLED)
					{
						Log("Files to process:\r\n" + string.Join("\r\n", files.ToArray()));
					}

					foreach (string file in files)
					{
						try
						{
							string path = Path.Combine(folder, file);
							if (File.Exists(path))
							{
								analyzer.ProcessFile(path);
							}
						}
						catch (Exception ex)
						{
							string message = string.Format("Ошибка при обработке файла '{0}'.", file);
							if (LOG_ENABLED)
							{
								Log(string.Format("{0}\r\n{1}", message, ex), true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log(ex.ToString(), true);
				//MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void AnalyzerWin1251Finded(string file, CharsetDetector detector)
		{
			Log(file);
			ConvertFile(file);
		}

		private static void AnalyzerOnError(string file, Exception exception, CharsetDetector detector)
		{
			Log(string.Format("Ошибка {0}:\r\n{1}\r\n", file, exception), true);
			//MessageBox.Show(file + ":\r\n\r\n" + exception, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void Log(string message, bool error = false, bool emptyLine = false)
		{
			string text = string.Format("\r\n[{0:dd.MM.yyyy HH.mm:ss} {1}] {2}", DateTime.Now, logPrefix, message);
			if (emptyLine)
			{
				text = "\r\n" + text;
			}

			string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			File.AppendAllText(logFile, text, Encoding.UTF8);

			if (error)
			{
				string errorLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errors.txt");
				File.AppendAllText(errorLogFile, text, Encoding.UTF8);
			}
		}
	}
}
