using MediatR;

namespace ConsoleDIPlayground.Shared;

/// <summary>
/// Common base for events that provides commodity properties.
/// </summary>
public abstract class BaseEvent : INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BaseEvent"/> class.
  /// </summary>
  /// <param name="sender">Instance of object in which the event ocurred.</param>
  protected BaseEvent(object sender)
  {
    Sender = Guard.Against.Null(sender, nameof(sender));
    Date = DateTimeOffset.UtcNow;
    Id = Guid.NewGuid();
  }

  /// <summary>
  /// Gets or sets the date when the event ocurred. By default, value is <see cref="DateTimeOffset.UtcNow"/>.
  /// </summary>
  public DateTimeOffset Date { get; protected set; }

  /// <summary>
  /// Gets the Id of the event raised.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Gets the instance of the object in which the event ocurred.
  /// </summary>
  public object Sender { get; init; }
}
