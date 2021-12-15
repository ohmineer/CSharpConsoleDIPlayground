using ConsoleDIPlayground.Core;
using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Console;

public class UserMessageEventHandler : INotificationHandler<UserMessageEvent>
{
  public Task Handle(UserMessageEvent notification, CancellationToken cancellationToken)
  {
    if (!cancellationToken.IsCancellationRequested && !string.IsNullOrWhiteSpace(notification.Message))
    {
      switch (notification.Inline)
      {
        case false:
          AnsiConsole.MarkupLine(notification.Message);
          break;
        case true:
          AnsiConsole.Markup(notification.Message);
          break;
      }
    }

    return Task.CompletedTask;
  }
}
