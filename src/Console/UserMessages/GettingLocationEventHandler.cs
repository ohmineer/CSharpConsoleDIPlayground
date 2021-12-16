using ConsoleDIPlayground.Core;
using ConsoleDIPlayground.Infrastructure;
using ConsoleDIPlayground.Shared;
using MediatR;
using Spectre.Console;

namespace ConsoleDIPlayground.Console;

public class GettingLocationEventHandler : INotificationHandler<GettingLocationEvent>
{
  private readonly ILogger<GettingLocationEventHandler> _logger;

  public GettingLocationEventHandler(ILogger<GettingLocationEventHandler> logger)
  {
    _logger = Guard.Against.Null(logger);
  }

  public async Task Handle(GettingLocationEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Event {@Event} received", notification);

    if (cancellationToken.IsCancellationRequested)
    {
      _logger.LogDebug("Cancellation requested by user");
      return;
    }

    await AnsiConsole
      .Status()
      .Spinner(Spinner.Known.BouncingBall)
      .StartAsync(
        "🌍 [red]Getting current location...[/]",
        async _ => await SimulateApiConnection.Connect(5000, cancellationToken));

    _logger.LogDebug("Event {@Event} successfully processed", notification);
  }
}
