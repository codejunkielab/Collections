namespace CodeJunkie.Collections;

/// <summary>
/// Definition of task start status processing
/// </summary>
public enum StartTaskStatus : byte {
  /// <summary>
  /// The process can be completed immediately
  /// </summary>
  Done = 0,

  /// <summary>
  /// The process can continue
  /// </summary>
  CanResume,

  /// <summary>
  /// The process cannot continue. It is necessary to wait for other tasks to complete
  /// </summary>
  HasToWait,

  /// <summary>
  /// The process cannot continue due to an unknown error
  /// </summary>
  UnknownError
}
