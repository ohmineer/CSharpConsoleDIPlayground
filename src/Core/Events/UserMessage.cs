using MediatR;

namespace ConsoleDIPlayground;

public class UserMessage : INotification
{
  public UserMessage(string message, bool inline = false)
  {
    Message = message;
    Inline = inline;
  }

  public bool Inline { get; init; }

  public string Message { get; init; }
}
