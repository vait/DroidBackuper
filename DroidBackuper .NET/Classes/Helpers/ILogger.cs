namespace DroidBackuper.NET.Classes.Helpers
{
    interface ILogger
    {
        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="message">сообщение</param>
        void WriteLog(string message);

        /// <summary>
        /// Записать строку с форматированием в лог
        /// </summary>
        /// <param name="format">строка с форматом</param>
        /// <param name="args">параметры</param>
        void WriteLog(string format, params object[] args);
    }
}
