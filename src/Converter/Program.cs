using System;
using System.Threading;
using System.Windows.Forms;

namespace Converter
{
	static class Program
	{
		private static bool ShowMainForm = false;

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
					UtilApp.Process(args);
					return;
				}
				MessageBox.Show(args[0]);
				return;
			}

			Application.ThreadException += OnApplicationOnThreadException;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (ShowMainForm)
			{
				Application.Run(new MainForm());
				//Tests.ConsoleRedirect.Test();
			}
			else
			{
				Application.Run(new ConsoleForm());
			}
		}

		private static void OnApplicationOnThreadException(object sender, ThreadExceptionEventArgs args)
		{
			MessageBox.Show(args.Exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
