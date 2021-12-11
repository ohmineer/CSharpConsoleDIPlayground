using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleDIPlayground;

public class AppOptions
{
  [NotNull]
  [RegularExpression(@"^https?:\/\/\w+(\.\w+)*(:[0-9]+)?(\/.*)?$")]
  public string? LocationApiUrl { get; set; }
}
