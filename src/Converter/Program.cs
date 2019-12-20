using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
