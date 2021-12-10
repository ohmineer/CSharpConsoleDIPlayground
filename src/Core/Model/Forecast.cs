namespace ConsoleDIPlayground;

public class Forecast
{
  public static Forecast Default =>
    new() { City = "Undefined", CurrentWeather = "Unknown", TemperatureC = 0, };

  public string City { get; set; } = string.Empty;

  public string CurrentWeather { get; set; } = string.Empty;

  public int TemperatureC { get; set; }
}
