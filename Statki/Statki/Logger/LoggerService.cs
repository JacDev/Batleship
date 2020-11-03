using Battleship.Interfaces;
using System;
using System.IO;

namespace Battleship.Logger
{
	enum LogLevel { Info, Warning, Error };
	public  class LoggerService : ILoggerService
	{
		private static void WriteToFile(Exception ex, LogLevel logLevel)
		{
			string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
			string filePath = Path.Combine(basePath, "logs.txt");

			StreamWriter logFile;
				logFile = File.AppendText(filePath);
			logFile.WriteLine("----------------------------------------------------------------------");
			logFile.WriteLine($"Time - {DateTime.Now} | Level - {Enum.GetName(typeof(LogLevel), logLevel)} | ");
			logFile.WriteLine("----------------------EXCEPTION DETAILS-------------------------------");
			logFile.Write("StackTrace - {0} | ", ex.StackTrace);
			logFile.Write("Message - {0} |  ", ex.Message);
			logFile.Write("Source - {0} | ", ex.Source);
			logFile.WriteLine("InnerException - {0} |", ex.InnerException);

			logFile.WriteLine("----------------------------------------------------------------------");
			logFile.WriteLine();
			logFile.Close();
		}
		public void Warning(Exception ex)
		{
			WriteToFile(ex, LogLevel.Warning);
		}
		public void Info(Exception ex)
		{
			WriteToFile(ex, LogLevel.Info);
		}
		public void Error(Exception ex)
		{
			WriteToFile(ex, LogLevel.Error);
		}
	}
}
