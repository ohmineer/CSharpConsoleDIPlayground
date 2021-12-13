using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Shared;
using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Console;

public class LocationFoundEventHandler : INotificationHandler<LocationFoundEvent>
{
  private readonly ILogger<LocationFoundEventHandler> _logger;

  public LocationFoundEventHandler(ILogger<LocationFoundEventHandler> logger)
  {
    _logger = Guard.Against.Null(logger);
  }

  public Task Handle(LocationFoundEvent notification, CancellationToken cancellationToken)
  {
    Guard.Against.Cancellation(cancellationToken);
    _logger.LogDebug("Event {@Event} received", notification);

    AnsiConsole.MarkupLine($"[green]Current location found:[/] [yellow]{notification.Location.City}[/] 🌎");

    _logger.LogDebug("Event {@Event} successfully processed", notification);
    return Task.CompletedTask;
  }
}
