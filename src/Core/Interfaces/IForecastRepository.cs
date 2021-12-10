namespace ConsoleDIPlayground;

public interface IForecastRepository
{
  Task FetchCurrentForecastData(CancellationToken token);

  Task<Forecast> GetCurrentForecastByCityName(string cityName);
}
