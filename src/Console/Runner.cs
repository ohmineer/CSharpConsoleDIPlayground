using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleDIPlayground;

public class Runner
{
  private readonly IHostApplicationLifetime _hostLifeTime;
  private readonly ILogger _logger;
  private readonly IServiceProvider _serviceProvider;

  public Runner(
    IServiceProvider serviceProvider,
    ILogger<Runner> logger,
    IHostApplicationLifetime hostLifeTime)
  {
    Id = Guid.NewGuid();

    _logger = Guard.Against.Null(logger);
    _serviceProvider = Guard.Against.Null(serviceProvider, nameof(serviceProvider));
    _hostLifeTime = Guard.Against.Null(hostLifeTime, nameof(hostLifeTime));

    _hostLifeTime.ApplicationStarted.Register(OnStarted);
    _hostLifeTime.ApplicationStopped.Register(OnStopped);
  }

  public Guid Id { get; init; }

  public async Task RunAsync(CancellationToken token)
  {
    if (token.IsCancellationRequested)
    {
      await StopApplication(token);
    }

    while (!token.IsCancellationRequested)
    {
      try
      {
        await using AsyncServiceScope scope = _serviceProvider.CreateAsyncScope();
        _logger.LogDebug("------- Scope Begin -------");
        IForecastService forecastService =
          ActivatorUtilities.GetServiceOrCreateInstance<IForecastService>(scope.ServiceProvider);
        await forecastService.GetForecastInCurrentLocationAsync(token);
        await forecastService.GetForecastInCurrentLocationAsync(token);
        _logger.LogDebug("-------- Scope End --------");
      }
      catch (TaskCanceledException)
      {
        _logger.LogInformation("Application stopped by user");
        await StopApplication(token);
      }
    }
  }

  private void OnStarted() => _logger.LogDebug("{Name} with ID {Id} started", ToString(), Id);

  private void OnStopped() => _logger.LogDebug("{Name} with ID {Id} stopped", ToString(), Id);

  private Task StopApplication(CancellationToken token)
  {
    _hostLifeTime.StopApplication();
    return Task.FromCanceled(token);
  }
}
