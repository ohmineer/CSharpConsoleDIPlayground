using ConsoleDIPlayground.Shared;

namespace ConsoleDIPlayground.Core;

public class LocationFoundEvent : BaseEvent
{
  public LocationFoundEvent(object sender, Location location)
    : base(sender)
  {
    Location = Guard.Against.Null(location, nameof(location));
  }

  public Location Location { get; init; }
}
