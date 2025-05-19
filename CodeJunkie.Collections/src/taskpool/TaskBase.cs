namespace CodeJunkie.Collections;

/// <summary>
/// Base class for tasks
/// </summary>
public abstract class TaskBase : IReference {
  /// <summary>
  /// Default priority of the task
  /// </summary>
  public const int _defaultPriority = 0;

  /// <summary>
  /// Default constructor
  /// </summary>
  public TaskBase() {
    SerialId = 0;
    Tag = null;
    Priority = _defaultPriority;
    Done = false;
    Userdata = null;
  }

  /// <summary>
  /// Returns the serial number of the task
  /// </summary>
  public int SerialId { get; private set; }

  /// <summary>
  /// Returns the tag label of the task
  /// </summary>
  public string? Tag { get; private set; }

  /// <summary>
  /// Returns the priority of the task
  /// </summary>
  public int Priority { get; private set; }

  /// <summary>
  /// Returns the user data of the task
  /// </summary>
  public object? Userdata { get; private set; }

  /// <summary>
  /// Accessor and mutator for whether the task is completed
  /// </summary>
  public bool Done { get; set; }

  /// <summary>
  /// Returns the description of the task
  /// </summary>
  public virtual string? Description => null;

  /// <summary>
  /// Initializes the task
  /// </summary>
  /// <param name="serialId">Serial number of the task</param>
  /// <param name="tag">Tag label of the task</param>
  /// <param name="priority">Priority of the task</param>
  /// <param name="userData">User data of the task</param>
  public void Initialize(int serialId, string? tag, int priority, object? userData) {
    SerialId = serialId;
    Tag = tag;
    Priority = priority;
    Userdata = userData;
    Done = false;
  }

  /// <summary>
  /// Cleans up the task
  /// </summary>
  public virtual void Clear() {
    SerialId = 0;
    Tag = null;
    Priority = _defaultPriority;
    Userdata = null;
    Done = false;
  }
}
