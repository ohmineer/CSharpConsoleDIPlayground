namespace ConsoleDIPlayground.Infrastructure;

public static class SimulateApiConnection
{
  public static async Task Connect(int maxRandomMsDelay, CancellationToken cancellationToken)
  {
    Guard.Against.NegativeOrZero(maxRandomMsDelay, nameof(maxRandomMsDelay), "Delay must be positive");
    Guard.Against.Cancellation(cancellationToken);

    try
    {
      await Task.Delay(new Random().Next(maxRandomMsDelay), cancellationToken);
    }
    catch (TaskCanceledException)
    {
      await Task.FromCanceled(cancellationToken);
    }
  }
}
