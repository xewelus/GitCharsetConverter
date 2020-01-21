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
					Tests.ProcessFolder(folder);
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

		private static void OnApplicationOnThreadException(object sender, ThreadExceptionEventArgs args)
		{
			MessageBox.Show(args.Exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
