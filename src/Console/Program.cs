using ConsoleDIPlayground;
using ConsoleDIPlayground.Console;
using ConsoleDIPlayground.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using CancellationTokenSource cts = new();

ConfigureCancellationOnUserRequest(cts);
WriteTitle();

using IHost host = ConsolePlaygroundHostBuilder.Build(args);
_ = host.RunAsync(cts.Token);

try
{
  // A scope is required so that Runner class can use scoped lifetime services.
  await using AsyncServiceScope scope = host.Services.CreateAsyncScope();

  // Main logic of the app is in "RunAsync" method of "Runner" instance.
  Runner runner = ActivatorUtilities.GetServiceOrCreateInstance<Runner>(scope.ServiceProvider);
  await runner.RunAsync(cts.Token);
}
catch (TaskCanceledException)
{
  AnsiConsole.Write("\n\n");
}

await host.StopAsync(cts.Token);

// Not happy with this line. Need to understand how to know whether AnsiConsole is being used by another thread and
// wait for its release in that case.
await Task.Delay(500);
WriteFarewell();
await Task.Delay(500);

// END of application.

/// <summary>
/// When user presses Ctrl + C, cancelation will be requested. Methods should be ready to take this
/// request and throw a TaskCanceled exception that will stop the application.
/// </summary>
/// <param name="cts">Global/Shared cancellation source for stopping the app</param>
static void ConfigureCancellationOnUserRequest(CancellationTokenSource cts)
{
  Console.CancelKeyPress += (_, e) =>
  {
    cts.Cancel();
    e.Cancel = true;
  };
}

/// <summary>
/// Displays the title of the app when it is started
/// </summary>
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

///
/// <summary>
///  Shows a "pretty" message indicating user has terminated the application.
/// </summary>
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
