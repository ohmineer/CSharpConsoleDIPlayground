using ConsoleDIPlayground.Shared;

namespace ConsoleDIPlayground.Core;

public class GettingLocationEvent : BaseEvent
{
  public GettingLocationEvent(object sender)
        : base(sender)
  {
  }
}
