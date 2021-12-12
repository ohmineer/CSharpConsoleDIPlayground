using ConsoleDIPlayground.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleDIPlayground.Console;

/// <summary>
/// Provides the method <see cref="RunAsync"/> which contains the main logic of the application.
/// </summary>
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

  /// <summary>
  /// Gets the ID of the <see cref="Runner"/> instance for debugging purposes.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Contains the main logic of the app.
  /// </summary>
  /// <param name="token">Token used to signal user wants to end the application.</param>
  /// <returns>An awaitable task when infinite loop is canceled by user.</returns>
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
        // This loop is just created to demonstrate how scoped services are instantiated everytime
        // a new scope is created. This simple app probably does not need scopes but we just came to play 🔥🔥.
        // GetForecastInCurrentLocationAsync method is called twice per scope created.
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

  /// <summary>
  /// Performs actions when the hosted application starts through the <see cref="IHostApplicationLifetime"/> interface.
  /// </summary>
  private void OnStarted() => _logger.LogDebug("{Name} with ID {Id} started", ToString(), Id);

  /// <summary>
  /// Performs actions when the hosted application stops through the <see cref="IHostApplicationLifetime"/> interface.
  /// </summary>
  private void OnStopped() => _logger.LogDebug("{Name} with ID {Id} stopped", ToString(), Id);

  /// <summary>
  /// Performs the common operations needed when the application is stopped by the user.
  /// </summary>
  /// <param name="token">Token used to signal user wants to end the application.</param>
  /// <returns>An awaitable task when infinite loop is canceled by user.</returns>
  private Task StopApplication(CancellationToken token)
  {
    _hostLifeTime.StopApplication();
    return Task.FromCanceled(token);
  }
}
