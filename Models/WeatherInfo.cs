namespace WeatherApp.Models;

public class WeatherInfo
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public int Pressure { get; set; }
    public double WindSpeed { get; set; }
    public string IconUrl { get; set; } = string.Empty;
}

public class WeatherResult
{
    public bool Success { get; private init; }
    public string? Error { get; private init; }
    public WeatherInfo? Data { get; private init; }

    public static WeatherResult Ok(WeatherInfo data) => new() { Success = true, Data = data };

    public static WeatherResult Fail(string error) => new() { Success = false, Error = error };
}
