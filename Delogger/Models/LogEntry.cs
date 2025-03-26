namespace Delogger.Models;

public class LogEntry
{
    [JsonProperty(nameof(Timestamp))]
    public DateTime Timestamp { get; set; } = default;

    [JsonProperty(nameof(Level))]
    public string Level { get; set; } = string.Empty;

    [JsonProperty(nameof(Exception))]
    public string Exception { get; set; } = string.Empty;

    [JsonProperty(nameof(MessageTemplate))]
    public string MessageTemplate { get; set; } = string.Empty;

    [JsonProperty(nameof(TraceId))]
    public string TraceId { get; set; } = string.Empty;

    [JsonProperty(nameof(SpanId))]
    public string SpanId { get; set; } = string.Empty;

    [JsonProperty(nameof(Properties))]
    public Dictionary<string, object> Properties { get; set; } = [];

    [JsonIgnore]
    public string RawLogEntry { get; set; } = string.Empty;
    
    [JsonIgnore]
    public int EntryId { get; set; } = 0;
}
