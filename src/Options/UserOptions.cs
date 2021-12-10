using System.Diagnostics.CodeAnalysis;

namespace ConsoleDIPlayground;

public class UserOptions
{
  [NotNull]
  public string? ApiKey { get; set; }

  [NotNull]
  public string? MachineName { get; set; }

  [NotNull]
  public string? UserName { get; set; }
}
