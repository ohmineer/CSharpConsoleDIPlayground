using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Core;

public class UserMessageNotificationHandler : INotificationHandler<UserMessage>
{
  public Task Handle(UserMessage notification, CancellationToken cancellationToken)
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
