using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground;

public class ForecastService : BaseService<ForecastService>, IForecastService
{
  private readonly IDateProviderService _dateProviderService;
  private readonly ILocationService _locationService;
  private readonly IForecastRepository _repository;

  public ForecastService(
    ILogger<ForecastService> logger,
    IMediator mediator,
    ILocationService locationService,
    IForecastRepository repository,
    IDateProviderService dateProviderService)
    : base(logger, mediator)
  {
    _repository = Guard.Against.Null(repository, nameof(repository));
    _locationService = Guard.Against.Null(locationService, nameof(locationService));
    _dateProviderService = Guard.Against.Null(dateProviderService, nameof(dateProviderService));
  }

  public async Task<Forecast> GetForecastAsync(Location? location, CancellationToken token)
  {
    Guard.Against.Cancellation(token);
    Guard.Against.Null(location, nameof(location));

    Logger.LogInformation("Getting forecast from {@Location}", location);

    await SimulateApiConnection.Connect(1000, token);

    string currentDate = _dateProviderService.GetCurrentDate().ToShortDateString();

    Forecast forecastResponse =
      await _repository.GetCurrentForecastByCityName(location.City);

    Logger.LogInformation("Forecast retrieved {@Forecast}", forecastResponse);

    await Mediator.Publish(
      new UserMessage(
        $"Weather in [yellow]{forecastResponse.City}[/]: " +
        $"🌡️[blue]{forecastResponse.TemperatureC} degC[/], " +
        $"[blue]{forecastResponse.CurrentWeather}[/] {GetForecastEmoji(forecastResponse)} " +
        $"(Updated 📆: [blue]{currentDate}[/])" +
        $"{Environment.NewLine}"),
      token);

    return forecastResponse;
  }

  public async Task<Forecast> GetForecastInCurrentLocationAsync(CancellationToken token)
  {
    Location currentLocation = await _locationService.GetCurrentLocationAsync(token);
    return await GetForecastAsync(currentLocation, token);
  }

  private static string GetForecastEmoji(Forecast forecastResponse) => forecastResponse.CurrentWeather switch
  {
    "Sunny" => "🌞🌞",
    "Cloudy" => "☁️ ☁️",
    "Rainy" => "🌧️ 🌧️",
    "Windy" => " 🎏",
    _ => string.Empty,
  };
}
