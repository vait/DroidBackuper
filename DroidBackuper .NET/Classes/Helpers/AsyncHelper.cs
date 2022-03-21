using System.Threading.Tasks;

namespace DroidBackuper.NET.Classes.Helpers
{
	internal static class AsyncHelper
	{
		public static void FireAndForgot(this Task task, ILogger logger)
		{
			task.ContinueWith(r =>
			{
				if (r.IsFaulted)
				{
					logger.WriteLog(r.Exception.Message);
				}
			});
		}
	}
}
