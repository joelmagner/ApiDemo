namespace MiniGram.Api.Configuration;

public class AppSettings
{
    public Logging Logging { get; set; } = new();
    public JwtConfig Jwt { get; set; } = new();
}

public class Logging
{
    public LogLevel LogLevel { get; set; } = new();
}

public class LogLevel
{
    public string Default { get; set; } = "Information";
    public string MicrosoftAspNetCore { get; set; } = "Warning";
}