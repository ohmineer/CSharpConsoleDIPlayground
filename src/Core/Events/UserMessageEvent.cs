using MediatR;

namespace ConsoleDIPlayground.Core;

public class UserMessageEvent : INotification
{
  public UserMessageEvent(string message, bool inline = false)
  {
    Message = message;
    Inline = inline;
  }

  public bool Inline { get; init; }

  public string Message { get; init; }
}
