namespace CodeJunkie.Collections;

/// <summary>
/// Task status enumeration.
/// </summary>
public enum TaskStatus : byte {
  /// <summary>
  /// The task is not started yet.
  /// </summary>
  Todo = 0,

  /// <summary>
  /// The task is in progress.
  /// </summary>
  Doing,

  /// <summary>
  /// The task is completed.
  /// </summary>
  Done
}
