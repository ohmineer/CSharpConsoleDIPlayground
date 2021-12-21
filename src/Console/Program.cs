using ConsoleDIPlayground.Console;
using ConsoleDIPlayground.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

SetDisplay();
WriteTitle();

IHost? host = default;
CancellationTokenSource cancellationTokenSource = new();
CancellationToken cancellationToken = cancellationTokenSource.Token;
ConfigureCancellationOnUserRequest(cancellationTokenSource);

try
{
  host = ConsolePlaygroundHostBuilder.Build(args);
  await host.StartAsync(cancellationToken).ConfigureAwait(false);

  // A scope is required so that Runner class can use scoped lifetime services.
  await using (AsyncServiceScope scope = host.Services.CreateAsyncScope())
  {
    // Main logic of the app is in "RunAsync" method of "Runner" instance.
    Runner runner = ActivatorUtilities.GetServiceOrCreateInstance<Runner>(scope.ServiceProvider);
    await runner.RunAsync(cancellationToken).ConfigureAwait(false);
  }

  await host.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);
}
catch (TaskCanceledException)
{
  AnsiConsole.Write("\n\n");
}
catch (Exception ex)
{
  AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
}
finally
{
  cancellationTokenSource?.Dispose();

  if (host is IAsyncDisposable disposableHost)
  {
    await disposableHost.DisposeAsync().ConfigureAwait(false);
  }

  WriteFarewell();
  await Task.Delay(500);
}

// END of application.

/// <summary>
/// When user presses Ctrl + C, cancelation will be requested. Methods should be ready to take this
/// request and throw a TaskCanceled exception that will stop the application.
/// </summary>
/// <param name="cancellationTokenSource">Global/Shared cancellation source for stopping the app</param>
static void ConfigureCancellationOnUserRequest(CancellationTokenSource cancellationTokenSource)
{
  Console.CancelKeyPress += (_, e) =>
  {
    cancellationTokenSource.Cancel();
    e.Cancel = true;
  };
}

/// <summary>
/// Configures the display.
/// </summary>
static void SetDisplay()
{
  AnsiConsole.Reset();
  AnsiConsole.Clear();
  AnsiConsole.Profile.Width = 800;
  AnsiConsole.Profile.Height = 600;
}

/// <summary>
/// Displays the title of the app when it is started.
/// </summary>
static void WriteTitle()
{
  Markup content = new Markup(
    "\n[bold yellow on black]WEATHER FORECAST SERVICE SIMULATOR[/]\n\n" +
    "[blue]A .Net 6.0 console app to learn about dependency injection[/]\n\n" +
    $"[bold]Version:[/] [red]{ThisAssembly.AssemblyFileVersion}[/]    " +
    $"[bold]Date:[/] [red]{ThisAssembly.GitCommitDate.ToShortDateString()}[/]")
    .Centered();

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
