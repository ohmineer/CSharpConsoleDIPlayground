using System.Text.Json;
using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;
using Nito.AsyncEx;

namespace ConsoleDIPlayground.Infrastructure;

public class LocationRepository : ILocationRepository
{
  private static AsyncLazy<List<Location>> s_locationCollection = new(
    async () => await Task.FromResult(new List<Location>()));

  private readonly ILogger _logger;

  public LocationRepository(ILogger<LocationRepository> logger) => _logger = Guard.Against.Null(logger);

  public static LocationRepository Initialize(ILogger<LocationRepository> logger, CancellationToken token)
  {
    LocationRepository repository = new(logger);

    s_locationCollection = new AsyncLazy<List<Location>>(async () => await GetLocationListAsync(logger, token));
    return repository;
  }

  public async Task<Location> GetLocationByIndex(int cityIndex)
  {
    List<Location> locationCollection = await s_locationCollection;
    _logger.LogInformation("Location index provided: {Index}", cityIndex);

    if (cityIndex < locationCollection.Count && cityIndex >= 0)
    {
      return locationCollection[cityIndex];
    }

    _logger.LogError("Index provided out of range: {Index}", cityIndex);
    return Location.Default;
  }

  public async Task<Location> GetLocationByName(string cityName)
  {
    List<Location> locationCollection = await s_locationCollection;

    Location location = locationCollection.FirstOrDefault(
      l => l.City.Equals(cityName, StringComparison.OrdinalIgnoreCase),
      Location.Default);

    _logger.LogInformation("Location retrieved from repository: {@Location}", location);
    return location;
  }

  public async Task<int> GetLocationCount()
  {
    List<Location> locationCollection = await s_locationCollection;
    int count = locationCollection.Count;

    _logger.LogInformation("Location count retrieved from repository: {Count}", count);

    return count;
  }

  private static async Task<List<Location>> GetLocationListAsync(
    ILogger<LocationRepository> logger,
    CancellationToken token)
  {
    logger.LogInformation("Initialzing location repository");

    using StreamReader? sr = new(
      Path.Combine(AppUtilities.GetExecutingDirectory().FullName, "Data", "es-cities.json"));

    JsonSerializerOptions? serializeOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    List<Location>? locationList =
      await JsonSerializer.DeserializeAsync<List<Location>>(sr.BaseStream, serializeOptions, token);

    if (locationList is null)
    {
      throw new InvalidOperationException("Repository could not be read");
    }

    logger.LogInformation("Repository initialized");
    return locationList;
  }
}
