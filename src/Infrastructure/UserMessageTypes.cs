namespace ConsoleDIPlayground;

/// <summary>
/// Returns a <see cref="IUserMessageComposer"/> based on the message type.
/// </summary>
/// <param name="userMessageType">Type of message that composer returned will produce.</param>.
/// <returns>A message composer to customize the message that will be displayed to the user.</returns>
public delegate IUserMessageComposer UserMessageResolver(UserMessageTypes userMessageType);

/// <summary>
/// Types of messages that will be displayed to the user.
/// </summary>
public enum UserMessageTypes
{
  /// <summary>
  /// Type of message to display while application is searching for current user location.
  /// </summary>
  GetLocation,

  /// <summary>
  /// Type of message to display when user location has been found.
  /// </summary>
  LocationFound,

  /// <summary>
  /// Type of message to display when weather forecast has retrieved from corresponding service.
  /// </summary>
  ForecastRetrieved,
}
