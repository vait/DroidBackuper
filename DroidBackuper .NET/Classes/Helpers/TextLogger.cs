using System;
using System.Diagnostics;
using System.IO;

namespace DroidBackuper.NET.Classes.Helpers
{
	class TextLogger : ILogger
	{
		/// <summary>
		/// Записать сообщение в лог
		/// </summary>
		/// <param name="message">сообщение</param>
		public void WriteLog(string message)
		{
			System.Diagnostics.Trace.WriteLine(message);

		}

		/// <summary>
		/// Записать строку с форматированием в лог
		/// </summary>
		/// <param name="format">строка с форматом</param>
		/// <param name="args">параметры</param>
		public void WriteLog(string format, params object[] args)
		{
			WriteLog(String.Format(format, args));

		}

		public TextLogger()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			string appDir = Path.GetDirectoryName(new Uri(assembly.Location).LocalPath);
			string fileNameLog = "droidBackuper.log";
			string fullPathLog = Path.Combine(appDir, fileNameLog);
			Stream myFile = File.Open(fullPathLog, FileMode.Append, FileAccess.Write, FileShare.Read);

			if (!myFile.CanWrite)
			{
				System.Windows.MessageBox.Show("can't write");
			}

			TextWriterTraceListener textListener = new TextWriterTraceListener(myFile);
			textListener.IndentSize = 4;
			Trace.AutoFlush = true;

			Trace.Listeners.Add(textListener);
		}
	}
}
