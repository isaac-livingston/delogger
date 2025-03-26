namespace Delogger.Interfaces;
internal interface IDataService
{
    List<LogEntry> ReadLogFile(string logFilePath);
}
