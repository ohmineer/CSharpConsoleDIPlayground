
# Console Dependency Injection Playground in .NET 6

A toy application that aims to explore some of the depedency injection features of .NET 6 in development of console apps.

It simulates the connection to a weather forecast API which depends on additional services such as a current location API and an external date provider.

## Features
- Infinite loop simulating the process of getting the current location and then fetching available weather forecast data.
- Fake location, forecast and date provider services.
- Simple location and forecast data repositories loaded from local JSON files.
- A background (Hosted Service) which updates forecast repository from a local JSON file (Always the same but could be periodically downloaded / changed).
- Use of Microsoft's IOC container to:
    - Read configuration files, secrets and environment variables.
    - Validate and postconfigure options read from files.
    - Set up logging with Serilog.
    - Register concrete service implementations with different lifetime settings.
- [MediatR](https://github.com/jbogard/MediatR) to handle events ocurred in services.    
- [Spectre.Console](https://spectreconsole.net/) just to slightly prettify output text.
- [Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses) to guard against unexpected values in method parameters.
- [AsyncLazy from AsyncEx](https://github.com/StephenCleary/AsyncEx) to lazy loading location repository in an asynchronous manner (Not really needed as sample JSON source is small).
- Exploration of static analysis provided by NuGet packages:
    - [StyleCop.Analyzers](https://www.nuget.org/packages/StyleCop.Analyzers/)
    - [SonarAnalyzer.CSharp](https://www.nuget.org/packages/SonarAnalyzer.CSharp/)
    - [AsyncFixer](https://www.nuget.org/packages/AsyncFixer/)


## Screenshots

![App Screenshot](https://user-images.githubusercontent.com/8396492/145557125-b0e86066-0eea-4f5b-8ecf-1daefc0a6ee1.png)


## Future Work
- Add comments/documentation to the code.
- Forecast on user input.
- (Maybe) Connect to real services.
