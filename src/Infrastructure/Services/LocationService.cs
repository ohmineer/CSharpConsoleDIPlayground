using MediatR;
using Microsoft.Extensions.Options;

namespace ConsoleDIPlayground;

public class LocationService : BaseService<LocationService>, ILocationService
{
  private readonly AppOptions? _appOptions;
  private readonly ILocationRepository _locationRepository;
  private readonly UserOptions? _userOptions;

  public LocationService(
    IMediator mediator,
    ILogger<LocationService> logger,
    IOptions<AppOptions> appOptions,
    IOptionsMonitor<UserOptions> userOptions,
    ILocationRepository locationRepository)
    : base(logger, mediator)
  {
    _locationRepository = Guard.Against.Null(locationRepository, nameof(locationRepository));

    if (!appOptions.TryGetOptions(out _appOptions, out IEnumerable<string>? failures))
    {
      Mediator.Publish(new UserMessage(string.Join(" , ", failures)));
    }

    if (!userOptions.TryGetOptions(out _userOptions, out failures))
    {
      Mediator.Publish(new UserMessage(string.Join(" , ", failures)));
    }

    Guard.Against.Null(_appOptions, nameof(appOptions));
    Guard.Against.Null(_userOptions, nameof(userOptions));
  }

  public async Task<Location> GetCurrentLocationAsync(CancellationToken token)
  {
    Guard.Against.Cancellation(token);

    // Logging API key is a silly thing. DON'T DO IT.
    // This is just to prove we have read the configuration properly.
    Logger.LogDebug(
      "Connecting to location service at {LocationUrl} with credentials {UserName}:{ApiKey}",
      _appOptions?.LocationApiUrl,
      _userOptions?.UserName,
      _userOptions?.ApiKey);

    await Mediator.Publish(new UserMessage("🌍 [red]Getting current location...[/]    ", true), token);

    // Ideally this should be a connection to a GPS device from which coordinates are retrieved.
    // Then, a method call in location repository called GetLocationByCoordinates should be done.
    await SimulateApiConnection.Connect(1000, token);

    Location location = await _locationRepository.GetLocationByIndex(
      new Random().Next(0, await _locationRepository.GetLocationCount()));

    Logger.LogInformation("Current location: {@CurrentLocation}", location);
    await Mediator.Publish(
      new UserMessage($"[green]Current location found:[/] [yellow]{location.City}[/] 🌎"), token);

    return location;
  }
}
