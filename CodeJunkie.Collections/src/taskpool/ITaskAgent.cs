namespace CodeJunkie.Collections;

/// <summary>
/// Task Agent Interface
/// </summary>
/// <typeparam name="T">Type of the task</typeparam>
public interface ITaskAgent<T> where T : TaskBase {
  /// <summary>
  /// Returns the task
  /// </summary>
  T Task { get; }

  /// <summary>
  /// Initializes the task agent
  /// </summary>
  void Initialize();

  /// <summary>
  /// Polls the task agent
  /// </summary>
  /// <param name="elapseSeconds">Logical elapsed time (in seconds)</param>
  /// <param name="realElapseSeconds">Actual elapsed time (in seconds)</param>
  void Update(float elapseSeconds, float realElapseSeconds);

  /// <summary>
  /// Shuts down and cleans up the task agent
  /// </summary>
  void Shutdown();

  /// <summary>
  /// Starts processing the task
  /// </summary>
  /// <param name="task">The task to process</param>
  /// <returns>Returns the status of the task</returns>
  StartTaskStatus Start(T task);

  /// <summary>
  /// Stops the ongoing task and resets the task agent
  /// </summary>
  void Reset();
}
