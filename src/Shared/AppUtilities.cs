namespace ConsoleDIPlayground;

public static class AppUtilities
{
  public static DirectoryInfo GetExecutingDirectory() => new(AppDomain.CurrentDomain.BaseDirectory);
}
