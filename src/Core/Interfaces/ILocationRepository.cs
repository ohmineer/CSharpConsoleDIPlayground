namespace ConsoleDIPlayground.Core;

public interface ILocationRepository
{
  Task<Location> GetLocationByIndex(int cityIndex);

  Task<Location> GetLocationByName(string cityName);

  Task<int> GetLocationCount();
}
