namespace ConsoleDIPlayground;

public interface ILocationService : IBaseService
{
  Task<Location> GetCurrentLocationAsync(CancellationToken token);
}
