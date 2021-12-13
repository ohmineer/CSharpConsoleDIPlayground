using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;
using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Console;

public class ForecastRetrievedEventHandler : INotificationHandler<ForecastRetrievedEvent>
{
  private readonly ILogger<ForecastRetrievedEventHandler> _logger;

  public ForecastRetrievedEventHandler(ILogger<ForecastRetrievedEventHandler> logger)
  {
    _logger = Guard.Against.Null(logger);
  }

  public Task Handle(ForecastRetrievedEvent notification, CancellationToken cancellationToken)
  {
    Guard.Against.Cancellation(cancellationToken);

    _logger.LogDebug("Event {@Event} received", notification);

    string message =
      $"Weather in [yellow]{notification.Forecast.City}[/]: " +
      $"🌡️[blue]{notification.Forecast.TemperatureC} degC[/], " +
      $"[blue]{notification.Forecast.CurrentWeather}[/] {GetForecastEmoji(notification.Forecast)} " +
      $"(Updated 📆: [blue]{notification.ForecastUpdated.ToShortDateString()}[/])" +
      $"{Environment.NewLine}";

    AnsiConsole.MarkupLine(message);

    _logger.LogDebug("Event {@Event} successfully processed", notification);
    return Task.CompletedTask;
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
