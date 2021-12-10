using Microsoft.Extensions.Options;

namespace ConsoleDIPlayground;

public class UserOptionsValidator : IValidateOptions<UserOptions>
{
  public ValidateOptionsResult Validate(string name, UserOptions? options)
  {
    List<string>? failures = new();

    if (options is null)
    {
      return ValidateOptionsResult.Fail("User settings not imported");
    }

    if (string.IsNullOrWhiteSpace(options.UserName))
    {
      failures.Add("You need a user name");
    }

    if (string.IsNullOrWhiteSpace(options.ApiKey))
    {
      failures.Add("Location service requires a key");
    }

    if (failures.Count > 0)
    {
      return ValidateOptionsResult.Fail(failures);
    }

    return ValidateOptionsResult.Success;
  }
}
