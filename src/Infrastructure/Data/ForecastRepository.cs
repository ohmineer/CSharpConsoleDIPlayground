using System.Collections.Concurrent;
using System.Text.Json;

namespace ConsoleDIPlayground.Infrastructure;

public sealed class ForecastRepository : IForecastRepository
{
  private readonly ConcurrentDictionary<string, Forecast> _currentForecastCollection = new();

  private readonly Task _initialize;

  private readonly ILogger<ForecastRepository> _logger;

  public ForecastRepository(ILogger<ForecastRepository> logger)
  {
    _logger = Guard.Against.Null(logger);
    _initialize = FetchCurrentForecastData(CancellationToken.None);
  }

  public async Task FetchCurrentForecastData(CancellationToken token)
  {
    _logger.LogInformation("Updating current weather data...");

    JsonSerializerOptions? options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    using StreamReader? sr = new(
      Path.Combine(AppUtilities.GetExecutingDirectory().FullName, "Data", "es-weather.json"));

    List<Forecast>? weatherResponseList = await
      JsonSerializer.DeserializeAsync<List<Forecast>>(sr.BaseStream, options, token);

    if (weatherResponseList is null)
    {
      throw new InvalidOperationException("Can't access current weather data");
    }

    foreach (Forecast item in weatherResponseList)
    {
      if (!_currentForecastCollection.TryAdd(item.City, item))
      {
        _currentForecastCollection[item.City] = item;
      }
    }

    _logger.LogInformation("Weather data updated...");
  }

  public async Task<Forecast> GetCurrentForecastByCityName(string cityName)
  {
    await _initialize;

    if (_currentForecastCollection.TryGetValue(cityName, out Forecast? result))
    {
      return await Task.FromResult(result);
    }

    _logger.LogError("Forecast not found for {CityName}", cityName);
    return await Task.FromResult(Forecast.Default);
  }
}
