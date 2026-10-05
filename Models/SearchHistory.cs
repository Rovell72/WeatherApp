using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models;

public class SearchHistory
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public double Temperature { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
