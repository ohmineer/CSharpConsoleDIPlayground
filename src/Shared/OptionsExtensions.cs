using Microsoft.Extensions.Options;

namespace ConsoleDIPlayground.Shared;

public static class OptionsExtensions
{
  public static (T?, IEnumerable<string>) GetOptions<T>(this IOptions<T> optionsInterface) where T : class
  {
    optionsInterface.TryGetOptions(out T? options, out IEnumerable<string> failures);
    return (options, failures);
  }

  public static (T?, IEnumerable<string>) GetOptions<T>(this IOptionsMonitor<T> optionsInterface) where T : class
  {
    optionsInterface.TryGetOptions(out T? options, out IEnumerable<string> failures);
    return (options, failures);
  }

  public static T? GetOptions<T>(this IOptionsMonitor<T> optionsInterface, out IEnumerable<string> failures)
    where T : class
  {
    optionsInterface.TryGetOptions(out T? options, out failures);
    return options;
  }

  public static T? GetOptions<T>(this IOptions<T> optionsInterface, out IEnumerable<string> failures) where T : class
  {
    optionsInterface.TryGetOptions(out T? options, out failures);
    return options;
  }

  public static bool TryGetOptions<T>(
    this IOptions<T> optionsInterface,
    out T? options,
    out IEnumerable<string> failures)
    where T : class
  {
    try
    {
      options = optionsInterface.Value;
      failures = new List<string>();
      return true;
    }
    catch (OptionsValidationException ex)
    {
      options = null;
      failures = ex.Failures;
      return false;
    }
  }

  public static bool TryGetOptions<T>(
    this IOptionsMonitor<T> optionsMonitor,
    out T? options,
    out IEnumerable<string> failures)
    where T : class
  {
    try
    {
      options = optionsMonitor.CurrentValue;
      failures = new List<string>();
      return true;
    }
    catch (OptionsValidationException ex)
    {
      options = null;
      failures = ex.Failures;
      return false;
    }
  }
}
