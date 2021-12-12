namespace ConsoleDIPlayground.Core;

public interface IForecastService
{
  Task<Forecast> GetForecastAsync(Location location, CancellationToken token);

  Task<Forecast> GetForecastInCurrentLocationAsync(CancellationToken token);
}
