namespace App;

public class AppSettings
{
    [ConfigurationKeyName("Logging:LogLevel")]
    public Dictionary<string, string> LogLevel { get; set; } = new();
    public string Example { get; set; } = string.Empty;
}