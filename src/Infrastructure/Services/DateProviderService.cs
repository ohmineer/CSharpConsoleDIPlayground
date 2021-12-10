using MediatR;

namespace ConsoleDIPlayground;

public class DateProviderService : BaseService<DateProviderService>, IDateProviderService
{
  public DateProviderService(ILogger<DateProviderService> logger, IMediator mediator)
    : base(logger, mediator)
  {
    // Do something usefull here
  }

  public DateTime GetCurrentDate()
  {
    DateTime dateValue = DateTime.UtcNow;
    Logger.LogInformation("Date provider returned {Date}", dateValue);
    return dateValue;
  }
}
