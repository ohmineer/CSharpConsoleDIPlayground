namespace ConsoleDIPlayground.Shared;

public static class GuardExtensions
{
  public static void Cancellation(this IGuardClause _, CancellationToken token, string message = "Operation Canceled")
  {
    if (token.IsCancellationRequested)
    {
      throw new TaskCanceledException(message, null, token);
    }
  }

  public static ILogger<T> Null<T>(this IGuardClause _, ILogger<T>? logger) where T : class
  {
    if (logger is null)
    {
      return NullLogger<T>.Instance;
    }

    return logger;
  }
}
