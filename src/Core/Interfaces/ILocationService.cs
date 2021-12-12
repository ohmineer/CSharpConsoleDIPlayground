namespace ConsoleDIPlayground.Core;

public interface ILocationService : IBaseService
{
  Task<Location> GetCurrentLocationAsync(CancellationToken token);
}
