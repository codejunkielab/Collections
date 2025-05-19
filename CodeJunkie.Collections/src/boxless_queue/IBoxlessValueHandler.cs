namespace CodeJunkie.Collections;

/// <summary>
/// Represents a handler for processing values dequeued from a <see cref="BoxlessQueue"/>.
/// </summary>
public interface IBoxlessValueHandler {
  /// <summary>
  /// Handles a value dequeued from a <see cref="BoxlessQueue"/>.
  /// </summary>
  /// <param name="value">The dequeued value.</param>
  /// <typeparam name="TValue">The type of the dequeued value.</typeparam>
  void HandleValue<TValue>(in TValue value) where TValue : struct;
}
