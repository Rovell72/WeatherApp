using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class OpenWeatherResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sys")]
    public SysData? Sys { get; set; }

    [JsonPropertyName("main")]
    public MainData? Main { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherData> Weather { get; set; } = [];

    [JsonPropertyName("wind")]
    public WindData? Wind { get; set; }
}

public class SysData
{
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class WeatherData
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}
