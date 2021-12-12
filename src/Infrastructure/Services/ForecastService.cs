using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Infrastructure;

public class ForecastService : BaseService<ForecastService>, IForecastService
{
  private readonly IDateProviderService _dateProviderService;
  private readonly ILocationService _locationService;
  private readonly UserMessageResolver _messageResolver;
  private readonly IForecastRepository _repository;

  public ForecastService(
    ILogger<ForecastService> logger,
    IMediator mediator,
    ILocationService locationService,
    IForecastRepository repository,
    IDateProviderService dateProviderService,
    UserMessageResolver messageResolver)
    : base(logger, mediator)
  {
    _repository = Guard.Against.Null(repository, nameof(repository));
    _locationService = Guard.Against.Null(locationService, nameof(locationService));
    _dateProviderService = Guard.Against.Null(dateProviderService, nameof(dateProviderService));
    _messageResolver = Guard.Against.Null(messageResolver, nameof(messageResolver));
  }

  public async Task<Forecast> GetForecastAsync(Location? location, CancellationToken token)
  {
    Guard.Against.Cancellation(token);
    Guard.Against.Null(location, nameof(location));

    Logger.LogInformation("Getting forecast from {@Location}", location);

    await SimulateApiConnection.Connect(1000, token);

    Forecast forecastResponse =
      await _repository.GetCurrentForecastByCityName(location.City);

    Logger.LogInformation("Forecast retrieved {@Forecast}", forecastResponse);

    IUserMessageComposer forecastRetrievedUserMessage = _messageResolver(UserMessageTypes.ForecastRetrieved);
    await Mediator.Publish(
      forecastRetrievedUserMessage.Compose(forecastResponse, _dateProviderService.GetCurrentDate()),
      token);

    return forecastResponse;
  }

  public async Task<Forecast> GetForecastInCurrentLocationAsync(CancellationToken token)
  {
    Location currentLocation = await _locationService.GetCurrentLocationAsync(token);
    return await GetForecastAsync(currentLocation, token);
  }
}
