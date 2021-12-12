using ConsoleDIPlayground.Shared;

namespace ConsoleDIPlayground.Core;

public interface IDateProviderService : IBaseService
{
  DateTime GetCurrentDate();
}
