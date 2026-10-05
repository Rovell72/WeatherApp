using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Pages;

public class IndexModel(WeatherService weatherService, AppDbContext db, ILogger<IndexModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Введите название города")]
    [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
    public string? City { get; set; }

    public WeatherInfo? Weather { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? DatabaseWarning { get; private set; }

    public List<SearchHistory> History { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadHistoryAsync(cancellationToken);
    }

    public async Task OnPostAsync(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await weatherService.GetWeatherAsync(City!.Trim(), cancellationToken);

            if (result.Success)
            {
                Weather = result.Data;
                await SaveHistoryAsync(Weather!, cancellationToken);
            }
            else
            {
                ErrorMessage = result.Error;
            }
        }

        await LoadHistoryAsync(cancellationToken);
    }

    private async Task SaveHistoryAsync(WeatherInfo weather, CancellationToken cancellationToken)
    {
        try
        {
            db.SearchHistories.Add(new SearchHistory
            {
                City = weather.City,
                Temperature = weather.Temperature,
                Description = weather.Description,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Не удалось сохранить запись в историю");
            DatabaseWarning = "Не удалось сохранить запрос в базу данных.";
        }
    }

    private async Task LoadHistoryAsync(CancellationToken cancellationToken)
    {
        try
        {
            History = await db.SearchHistories
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Не удалось загрузить историю запросов");
            DatabaseWarning = "База данных недоступна, история запросов не отображается.";
        }
    }
}
