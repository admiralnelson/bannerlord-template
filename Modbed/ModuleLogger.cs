using System.IO;

namespace Modbed
{
	internal static class ModuleLogger
	{
		private static StreamWriter _streamWriter = new StreamWriter("logs/BattleTest.txt");

		public static StreamWriter Writer => _streamWriter;

		public static void Log(string format, params object[] args)
		{
			Writer.WriteLine(format, args);
			Writer.Flush();
		}
	}
}
