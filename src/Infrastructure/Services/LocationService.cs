using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;
using MediatR;
using Microsoft.Extensions.Options;

namespace ConsoleDIPlayground.Infrastructure;

public class LocationService : BaseService<LocationService>, ILocationService
{
  private readonly AppOptions? _appOptions;
  private readonly ILocationRepository _locationRepository;
  private readonly UserMessageResolver _messageResolver;
  private readonly UserOptions? _userOptions;

  public LocationService(
    IMediator mediator,
    ILogger<LocationService> logger,
    IOptions<AppOptions> appOptions,
    IOptionsMonitor<UserOptions> userOptions,
    ILocationRepository locationRepository,
    UserMessageResolver messageResolver)
    : base(logger, mediator)
  {
    _locationRepository = Guard.Against.Null(locationRepository, nameof(locationRepository));
    _messageResolver = Guard.Against.Null(messageResolver, nameof(messageResolver));

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

    // EventHandler in Mediator displays a progress that stops once location "has been found".
    // Strategy below allows canceling the task Mediator.Publish is running with both Ctrl + C and additional token.
    // This token is cancelled on every request after API simulator does its job.
    CancellationTokenSource mediatorCancellationSource = new();
    var combinedCancellationSource =
      CancellationTokenSource.CreateLinkedTokenSource(token, mediatorCancellationSource.Token);

    Task mediatorTask = Mediator.Publish(new GettingLocationEvent(this), combinedCancellationSource.Token);

    // Ideally this should be a connection to a GPS device from which coordinates are retrieved.
    // Then, a method call in location repository called GetLocationByCoordinates should be done.
    await SimulateApiConnection.Connect(5000, token);
    combinedCancellationSource.Cancel();
    await mediatorTask;

    combinedCancellationSource.Dispose();

    Location location = await _locationRepository.GetLocationByIndex(
      new Random().Next(0, await _locationRepository.GetLocationCount()));

    Logger.LogInformation("Current location: {@CurrentLocation}", location);

    await Mediator.Publish(new LocationFoundEvent(this, location), token);

    return location;
  }
}
