using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace ConsoleDIPlayground;

/// <summary>
/// Configures and builds the host used in the project.
/// </summary>
public static class ConsolePlaygroundHostBuilder
{
  /// <summary>
  /// Provides a method that configures and builds the Host used in this project.
  /// </summary>
  /// <remarks>
  /// Loads information from settings files and environment variables. Adds logging and register services with
  /// their appropriate lifetime.
  /// </remarks>
  /// <param name="args">Command line arguments passed when program is executed.</param>
  /// <returns>An initialized IHost.</returns>
  public static IHost Build(string[] args)
  {
    Guard.Against.Null(args, nameof(args));
    IHostBuilder builder = Host.CreateDefaultBuilder(args);

    AddConfiguration(builder, args);
    AddLogging(builder);
    AddOptions(builder);
    AddServices(builder);

    builder.UseConsoleLifetime();
    builder.ConfigureServices(s => s.BuildServiceProvider());

    return builder.Build();
  }

  /// <summary>
  /// Add configuration files and environment variables to the HostBuilder.
  /// </summary>
  /// <param name="hostBuilder">Shared HostBuilder under configuration.</param>
  /// <param name="args">Command line arguments passed when program is executed.</param>
  private static void AddConfiguration(IHostBuilder hostBuilder, string[] args) =>
    hostBuilder.ConfigureAppConfiguration(builder =>
    {
      Environment.SetEnvironmentVariable("EXECUTING_DIR", AppUtilities.GetExecutingDirectory().FullName);

      builder
        .SetBasePath(AppUtilities.GetExecutingDirectory().FullName)
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile(
          $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
          true,
          true)
        .AddEnvironmentVariables()
        .AddUserSecrets("bd63a892-7926-4c63-9425-678abb32fc50")
        .AddCommandLine(args);
    });

  /// <summary>
  /// Sets logging features.
  /// </summary>
  /// <param name="hostBuilder">Shared HostBuilder under configuration.</param>
  private static void AddLogging(IHostBuilder hostBuilder) =>
    hostBuilder.ConfigureLogging((loggingBuilder) =>
      {
        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Debug);
      })
      .UseSerilog((context, loggerConfiguration) =>
      {
        loggerConfiguration
          .ReadFrom.Configuration(context.Configuration, "Logging");
      });

  /// <summary>
  /// Configures and validates options read from configuration files.
  /// </summary>
  /// <param name="hostBuilder">Shared HostBuilder under configuration.</param>
  private static void AddOptions(IHostBuilder hostBuilder) =>
    hostBuilder
      .UseDefaultServiceProvider(serviceProviderOptions =>
      {
        serviceProviderOptions.ValidateOnBuild = true;
        serviceProviderOptions.ValidateScopes = true;
      })
      .ConfigureServices((context, services) =>
      {
        IConfiguration configuration = context.Configuration;

        services.AddOptions<AppOptions>()
          .Bind(configuration.GetSection(nameof(AppOptions)))
          .ValidateDataAnnotations()
          .ValidateOnStart();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<UserOptions>, UserOptionsValidator>());

        services.AddOptions<UserOptions>()
          .Bind(configuration.GetSection(nameof(UserOptions)))
          .Validate<IOptions<AppOptions>>(
            (userSettings, appConfig) =>
              !string.Equals(appConfig.Value.LocationApiUrl, @"http:\\OldApiLocation:3001")
              || userSettings.UserName != Environment.UserName, "Old Api version does not accept this kind of user!")
          .PostConfigure<IOptions<AppOptions>>((settings, appConfig) =>
          {
            if (string.IsNullOrWhiteSpace(settings.MachineName))
            {
              settings.MachineName = Environment.MachineName;
            }

            if (string.IsNullOrWhiteSpace(settings.UserName)
                && !string.Equals(appConfig.Value.LocationApiUrl, @"http:\\OldApiLocation:3001"))
            {
              settings.UserName = Environment.UserName;
            }
          })
          .ValidateOnStart();
      });

  /// <summary>
  /// Registers concrete implementations of services used by the application.
  /// </summary>
  /// <param name="hostBuilder">Shared HostBuilder under configuration.</param>
  private static void AddServices(IHostBuilder hostBuilder) =>
    hostBuilder
      .ConfigureServices(services =>
      {
        services.AddSingleton<ILocationRepository, LocationRepository>(sp =>
        {
          ILogger<LocationRepository> loggerService = sp.GetRequiredService<ILogger<LocationRepository>>();

          return LocationRepository.Initialize(
            loggerService,
            new CancellationTokenSource().Token);
        });

        services.AddSingleton<IForecastRepository, ForecastRepository>();

        services.AddHostedService<CurrentWeatherUpdateHostedService>();
        services.AddScoped<IDateProviderService, DateProviderService>();
        services.AddScoped<IForecastService, ForecastService>();
        services.AddScoped<ILocationService, LocationService>();

        services.AddTransient<Runner>();
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddScoped<GetLocationUserMessage>();
        services.AddScoped<ForecastUserMessage>();
        services.AddScoped<LocationFoundUserMessage>();

        services.AddScoped<UserMessageResolver>(serviceProvider => key =>
        {
          return key switch
          {
            UserMessageTypes.GetLocation => serviceProvider.GetRequiredService<GetLocationUserMessage>(),
            UserMessageTypes.LocationFound => serviceProvider.GetRequiredService<LocationFoundUserMessage>(),
            UserMessageTypes.ForecastRetrieved => serviceProvider.GetRequiredService<ForecastUserMessage>(),
            _ => throw new KeyNotFoundException()
          };
        });
      });
}
