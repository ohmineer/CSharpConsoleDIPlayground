using MediatR;

namespace ConsoleDIPlayground.Shared;

public abstract class BaseService<T> : IBaseService where T : class
{
  protected BaseService(ILogger<T> logger, IMediator mediator)
  {
    Logger = Guard.Against.Null(logger);
    Mediator = Guard.Against.Null(mediator, nameof(mediator));
    Id = Guid.NewGuid();

    Logger.LogDebug("Service {Name} with ID {Id}", ToString(), Id);
  }

  public Guid Id { get; init; }

  protected ILogger<T> Logger { get; init; }

  protected IMediator Mediator { get; init; }
}
