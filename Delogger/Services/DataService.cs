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
                try
                {
                    var logEntry = JsonConvert.DeserializeObject<LogEntry>(line);
                    if (logEntry != null)
                    {
                        logEntry.EntryId = entryId++;
                        logEntry.RawLogEntry = line; // Store the raw log entry for later use
                        logEntries.Add(logEntry);
                    }
                }
                catch (Exception)
                {
                    // Handle the case where a line could not be deserialized into a LogEntry.
                    // This could be due to malformed JSON. You can log this or skip the entry.
                    // For now, we will just ignore it and continue reading the next line.
                    // Optionally, you could log this to a separate error log for review.
                }
            }
        }

        return logEntries;
    }
}
