using ConsoleDIPlayground.Core;
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

  public Task Handle(GettingLocationEvent notification, CancellationToken cancellationToken)
  {
    _logger.LogDebug("Event {@Event} received", notification);

    if (cancellationToken.IsCancellationRequested)
    {
      _logger.LogDebug("Cancellation requested by user");
      return Task.CompletedTask;
    }

    AnsiConsole
      .Status()
      .Spinner(Spinner.Known.BouncingBall)
      .StartAsync(
        "🌍 [red]Getting current location...[/]",
        async _ =>
        {
          try
          {
            while (!cancellationToken.IsCancellationRequested)
            {
              await Task.Delay(100, cancellationToken);
            }
          }
          catch (TaskCanceledException)
          {
            _logger.LogDebug("Spinner cancelled");
          }
        });

    _logger.LogDebug("Event {@Event} successfully processed", notification);
    return Task.CompletedTask;
  }
}
