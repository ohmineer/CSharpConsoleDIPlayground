using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;

namespace ConsoleDIPlayground.Core;

public class ForecastRetrievedEvent : BaseEvent
{
  public ForecastRetrievedEvent(
    object sender,
    DateTime date,
    Forecast forecast)
    : base(sender)
  {
    ForecastUpdated = Guard.Against.OutOfRange(date, nameof(date), DateTime.MinValue, DateTime.UtcNow);
    Forecast = Guard.Against.Null(forecast, nameof(forecast));
  }

  public Forecast Forecast { get; init; }

  public DateTime ForecastUpdated { get; init; }
}
