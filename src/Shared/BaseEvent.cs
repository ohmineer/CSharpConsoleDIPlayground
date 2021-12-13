using MediatR;

namespace ConsoleDIPlayground.Shared;

/// <summary>
/// Common base for events that provides commodity properties.
/// </summary>
/// <typeparam name="T">Concrete class of the child event.</typeparam>
public abstract class BaseEvent<T> : INotification where T : class
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BaseEvent{T}"/> class.
  /// </summary>
  /// <param name="sender">Instance of object in which the event ocurred.</param>
  /// <param name="logger">Logger instance of <see cref="ILogger{T}"/>.</param>
  protected BaseEvent(object sender, ILogger<T> logger)
  {
    Sender = Guard.Against.Null(sender, nameof(sender));
    Logger = Guard.Against.Null(logger);

    Date = DateTimeOffset.UtcNow;
  }

  /// <summary>
  /// Gets or sets the date when the event ocurred. By default, value is <see cref="DateTimeOffset.UtcNow"/>.
  /// </summary>
  public DateTimeOffset Date { get; protected set; }

  /// <summary>
  /// Gets or sets the instance of the object in which the event ocurred.
  /// </summary>
  public object Sender { get; protected set; }

  protected ILogger<T> Logger { get; init; }
}
