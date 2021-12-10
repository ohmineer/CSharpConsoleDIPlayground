namespace ConsoleDIPlayground;

public interface IForecastService
{
  Task<Forecast> GetForecastAsync(Location location, CancellationToken token);

  Task<Forecast> GetForecastInCurrentLocationAsync(CancellationToken token);
}
