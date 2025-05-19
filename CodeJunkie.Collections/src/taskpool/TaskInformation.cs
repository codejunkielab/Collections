namespace CodeJunkie.Collections;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Task information.
/// This represents mission details.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct TaskInformation {
  private readonly bool _isValid;
  private readonly int _serialId;
  private readonly string? _tag;
  private readonly int _priority;
  private readonly object? _userData;
  private readonly TaskStatus _status;
  private readonly string? _description;

  /// <summary>
  /// Returns whether the task is valid.
  /// </summary>
  /// <returns>
  /// A boolean indicating validity:
  /// false - Returned when the task is invalid.
  /// true - Returned when the task is valid.
  /// </returns>
  public bool IsValid => _isValid;

  /// <summary>
  /// Returns the serial number of the task.
  /// </summary>
  /// <exception>Thrown when the task information is in an invalid state.</exception>
  public int SerialId {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _serialId;
    }
  }

  /// <summary>
  /// Returns the tag label of the task.
  /// </summary>
  /// <exception>Thrown when the task information is in an invalid state.</exception>
  public string? Tag {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _tag;
    }
  }

  /// <summary>
  /// Returns the priority of the task.
  /// </summary>
  /// <exception>Thrown when the task information is in an invalid state.</exception>
  public int Priority {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _priority;
    }
  }

  /// <summary>
  /// Returns the user data associated with the task.
  /// </summary>
  /// <exception>Thrown when the task information is in an invalid state.</exception>
  public object? Userdata {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _userData;
    }
  }

  /// <summary>
  /// Returns the status of the task.
  /// </summary>
  public TaskStatus Status {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _status;
    }
  }

  /// <summary>
  /// Returns the description of the task.
  /// </summary>
  public string? Description {
    get {
      if (!_isValid) {
        throw new InvalidOperationException("Invalid task");
      }

      return _description;
    }
  }

  /// <summary>
  /// Initializes task information.
  /// </summary>
  /// <param name="serialId">The serial number of the task.</param>
  /// <param name="tag">The tag label of the task.</param>
  /// <param name="priority">The priority of the task.</param>
  /// <param name="userData">The user data associated with the task.</param>
  /// <param name="status">The status of the task.</param>
  /// <param name="description">The description of the task.</param>
  public TaskInformation(int serialId,
                         string? tag,
                         int priority,
                         object? userData,
                         TaskStatus status,
                         string? description) {
    _isValid = true;
    _serialId = serialId;
    _tag = tag;
    _priority = priority;
    _userData = userData;
    _status = status;
    _description = description;
  }
}
