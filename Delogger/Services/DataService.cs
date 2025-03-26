namespace Delogger.Services;

internal class DataService : IDataService
{
    public List<LogEntry> ReadLogFile(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            throw new FileNotFoundException($"Log file not found: {logFilePath}");
        }

        var logEntries = new List<LogEntry>();

        using (var reader = new StreamReader(logFilePath))
        {
            string? line;
            var entryId = 0;

            while ((line = reader.ReadLine()) != null)
            {
                var logEntry = JsonConvert.DeserializeObject<LogEntry>(line);
                if (logEntry != null)
                {
                    logEntry.EntryId = entryId++;
                    logEntry.RawLogEntry = line; // Store the raw log entry for later use
                    logEntries.Add(logEntry);
                }
            }
        }

        return logEntries;
    }
}
