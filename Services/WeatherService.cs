using System.Net;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
{
    public async Task<WeatherResult> GetWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["OpenWeatherMap:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return WeatherResult.Fail("API-ключ не указан. Добавьте его в appsettings.json в раздел OpenWeatherMap:ApiKey");
        }

        var url = $"data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={Uri.EscapeDataString(apiKey)}&units=metric&lang=ru";

        try
        {
            using var response = await httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return WeatherResult.Fail($"Город \"{city}\" не найден. Проверьте правильность написания");
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return WeatherResult.Fail("Неверный API-ключ или ключ ещё не активирован");
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return WeatherResult.Fail("Превышен лимит запросов к сервису погоды. Попробуйте позже");
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Сервис погоды вернул код {StatusCode}", (int)response.StatusCode);
                return WeatherResult.Fail($"Сервис погоды вернул ошибку (код {(int)response.StatusCode}). Попробуйте позже");
            }

            var data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(cancellationToken);

            if (data?.Main is null || data.Weather.Count == 0)
            {
                return WeatherResult.Fail("Сервис не вернул данные о погоде для этого города");
            }

            var weather = data.Weather[0];
            var description = weather.Description;

            if (description.Length > 0)
            {
                description = char.ToUpper(description[0]) + description[1..];
            }

            return WeatherResult.Ok(new WeatherInfo
            {
                City = data.Name,
                Country = data.Sys?.Country ?? string.Empty,
                Temperature = data.Main.Temp,
                FeelsLike = data.Main.FeelsLike,
                Description = description,
                Humidity = data.Main.Humidity,
                Pressure = data.Main.Pressure,
                WindSpeed = data.Wind?.Speed ?? 0,
                IconUrl = $"https://openweathermap.org/img/wn/{weather.Icon}@2x.png"
            });
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Ошибка сети при обращении к сервису погоды");
            return WeatherResult.Fail("Не удалось подключиться к сервису погоды. Проверьте подключение к интернету");
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(ex, "Превышено время ожидания ответа сервиса погоды");
            return WeatherResult.Fail("Сервис погоды не ответил вовремя. Попробуйте ещё раз");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Ошибка разбора ответа сервиса погоды");
            return WeatherResult.Fail("Получен некорректный ответ от сервиса погоды");
        }
    }
}
