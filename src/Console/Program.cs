using ConsoleDIPlayground;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using CancellationTokenSource cts = new();

Console.CancelKeyPress += (_, e) =>
{
  cts.Cancel();
  e.Cancel = true;
};

WriteTitle();

using IHost host = ConsolePlaygroundHostBuilder.Build(args);
_ = host.RunAsync(cts.Token);

try
{
  await using AsyncServiceScope scope = host.Services.CreateAsyncScope();
  Runner runner = ActivatorUtilities.GetServiceOrCreateInstance<Runner>(scope.ServiceProvider);
  await runner.RunAsync(cts.Token);
}
catch (TaskCanceledException)
{
  AnsiConsole.Write("\n\n");
}

await host.StopAsync(cts.Token);
WriteFarewell();
await Task.Delay(500);

static void WriteTitle()
{
  Markup content = new Markup(
    "\n[bold yellow on black]WEATHER FORECAST SERVICE SIMULATOR[/]\n\n" +
    "[blue]A .Net 6.0 console app to learn about dependency injection[/]").Centered();

  Panel panel = new(content)
  {
    Header = new PanelHeader("[red]DI CONSOLE PLAYGROUND![/]").Centered(),
    Border = BoxBorder.Rounded,
    BorderStyle = Style.Plain,
    Expand = false,
  };

  AnsiConsole.Write(panel);
}

static void WriteFarewell()
{
  Markup content = new Markup(
    "[yellow]See you next time![/]".PadCenter(100)).Centered();

  Panel panel = new(content)
  {
    Header = new PanelHeader("[red]EXITING![/]").Centered(),
    Border = BoxBorder.Rounded,
    BorderStyle = Style.Plain,
    Expand = false,
  };

  _ = AnsiConsole.Prompt(new TextPrompt<string>("[green]Press enter key to exit[/]").AllowEmpty());
  AnsiConsole.WriteLine();
  AnsiConsole.Write(panel);
}
