namespace ConsoleDIPlayground.Console;

public class ForecastUserMessage : IUserMessageComposer
{
  public UserMessage Compose(params object[] p)
  {
    DateTime date = p.OfType<DateTime>().FirstOrDefault();
    Forecast forecast = p.OfType<Forecast>().FirstOrDefault(Forecast.Default);

    return new(
      $"Weather in [yellow]{forecast.City}[/]: " +
      $"🌡️[blue]{forecast.TemperatureC} degC[/], " +
      $"[blue]{forecast.CurrentWeather}[/] {GetForecastEmoji(forecast)} " +
      $"(Updated 📆: [blue]{date.ToShortDateString()}[/])" +
      $"{Environment.NewLine}");
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
