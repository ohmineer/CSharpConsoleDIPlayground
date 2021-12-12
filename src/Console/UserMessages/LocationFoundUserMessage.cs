namespace ConsoleDIPlayground.Console;

internal class LocationFoundUserMessage : IUserMessageComposer
{
  public UserMessage Compose(params object[] p)
  {
    Location location = p.OfType<Location>().FirstOrDefault(Location.Default);

    return new($"[green]Current location found:[/] [yellow]{location.City}[/] 🌎");
  }
}
