using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;
using Microsoft.Extensions.Hosting;

namespace ConsoleDIPlayground.Infrastructure;

public class CurrentWeatherUpdateHostedService : BackgroundService
{
  private readonly ILogger _logger;
  private readonly IForecastRepository _repository;

  public CurrentWeatherUpdateHostedService(
    ILogger<CurrentWeatherUpdateHostedService> logger,
    IForecastRepository forecastRepository)
  {
    _logger = Guard.Against.Null(logger);
    _repository = Guard.Against.Null(forecastRepository, nameof(forecastRepository));
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation("Executing background task: {Name}", ToString());
      await _repository.FetchCurrentForecastData(stoppingToken);
      _logger.LogInformation("Background task finished: {Name}", ToString());
      await Task.Delay(10000, stoppingToken);
    }
  }
}
