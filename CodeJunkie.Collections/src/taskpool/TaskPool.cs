namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a pool for managing tasks of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of tasks managed by the pool.</typeparam>
public sealed class TaskPool<T> where T : TaskBase {
  private readonly Stack<ITaskAgent<T>> _freeAgents;
  private readonly LinkedList<ITaskAgent<T>> _workingAgents;
  private readonly LinkedList<T> _waitingTasks;
  private bool _paused;
  private bool _canProcessNextTaskEnabled;

  /// <summary>
  /// Default constructor.
  /// </summary>
  public TaskPool() {
    _freeAgents = new Stack<ITaskAgent<T>>();
    _workingAgents = new LinkedList<ITaskAgent<T>>();
    _waitingTasks = new LinkedList<T>();
    _paused = false;
    _canProcessNextTaskEnabled = false;
  }

  /// <summary>
  /// Default constructor.
  /// </summary>
  public TaskPool(bool canProcessNextTaskEnabled) {
    _freeAgents = new Stack<ITaskAgent<T>>();
    _workingAgents = new LinkedList<ITaskAgent<T>>();
    _waitingTasks = new LinkedList<T>();
    _paused = false;
    _canProcessNextTaskEnabled = canProcessNextTaskEnabled;
  }

  /// <summary>
  /// Gets or sets whether the task pool is paused.
  /// </summary>
  public bool Paused {
    get => _paused;
    set => _paused = value;
  }

  /// <summary>
  /// Returns the total number of task agents.
  /// </summary>
  public int TotalAgentCount => FreeAgentCount + WorkingAgentCount;

  /// <summary>
  /// Returns the number of free task agents.
  /// </summary>
  public int FreeAgentCount => _freeAgents.Count;

  /// <summary>
  /// Returns the number of working task agents.
  /// </summary>
  public int WorkingAgentCount => _workingAgents.Count;

  /// <summary>
  /// Returns the number of waiting tasks.
  /// </summary>
  public int WaitingTaskCount => _waitingTasks.Count;

  /// <summary>
  /// </summary>
  public void Update(float elapseSeconds, float realElapseSeconds) {
    if (_paused) {
      return;
    }

    ProcessRunningTasks(elapseSeconds, realElapseSeconds);
    ProcessWaitingTasks(elapseSeconds, realElapseSeconds);
  }

  /// <summary>
  /// Shuts down the task pool and cleans up all resources.
  /// </summary>
  public void Shutdown() {
    RemoveAllTasks();

    while (FreeAgentCount > 0) {
      _freeAgents.Pop().Shutdown();
    }
  }

  /// <summary>
  /// Adds a task agent to the pool.
  /// </summary>
  /// <param name="agent">Agent to add to the pool.</param>
  public void AddAgent(ITaskAgent<T> agent) {
    if (agent == null) {
      throw new ArgumentNullException(nameof(agent), "Invalid agent");
    }

    agent.Initialize();
    _freeAgents.Push(agent);
  }

  /// <summary>
  /// Retrieves task information based on the given serial ID.
  /// </summary>
  /// <param name="serialId">Get the task information for this serial ID</param>
  /// <returns>Task information</returns>
  public TaskInformation GetTaskInformation(int serialId) {
    foreach (ITaskAgent<T> workingAgent in _workingAgents) {
      T workingTask = workingAgent.Task;
      if (workingTask.SerialId == serialId) {
        TaskStatus isDone = workingTask.Done ? TaskStatus.Done : TaskStatus.Doing;
        return new TaskInformation(
            workingTask.SerialId,
            workingTask.Tag,
            workingTask.Priority,
            workingTask.Userdata,
            isDone,
            workingTask.Description);
      }
    }

    foreach (T waitingTask in _waitingTasks) {
      if (waitingTask.SerialId == serialId) {
        return new TaskInformation(waitingTask.SerialId,
            waitingTask.Tag,
            waitingTask.Priority,
            waitingTask.Userdata,
            TaskStatus.Todo,
            waitingTask.Description);
      }
    }

    return default(TaskInformation);
  }

  /// <summary>
  /// Retrieves task information based on the specified tag label.
  /// </summary>
  /// <param name="tag">Get the task information for this tag label</param>
  /// <returns>Task information</returns>
  public TaskInformation[] CreateTaskInformations(string tag) {
    List<TaskInformation> results = new List<TaskInformation>();
    GetTaskInformations(tag, results);
    return results.ToArray();
  }

  /// <summary>
  /// Retrieves task information based on the specified tag label and stores it in the provided list.
  /// </summary>
  /// <param name="tag">Get the task information for this tag label</param>
  /// <param name="results">Output list for task information</param>
  /// <exception name="ArgumentNullException">Results argument is invalid</exception>
  public void GetTaskInformations(string tag, List<TaskInformation> results) {
    if (results == null) {
      throw new ArgumentNullException(nameof(results), "Invalid results argument");
    }

    results.Clear();
    foreach (ITaskAgent<T> workingAgent in _workingAgents) {
      T workingTask = workingAgent.Task;
      if (workingTask.Tag == tag) {
        TaskStatus isDone = workingTask.Done ? TaskStatus.Done : TaskStatus.Doing;
        TaskInformation newTaskInformation = new TaskInformation(workingTask.SerialId,
            workingTask.Tag,
            workingTask.Priority,
            workingTask.Userdata,
            isDone,
            workingTask.Description);
        results.Add(newTaskInformation);
      }
    }

    foreach (T waitingTask in _waitingTasks) {
      if (waitingTask.Tag == tag) {
        TaskInformation newTaskInformation = new TaskInformation(waitingTask.SerialId,
            waitingTask.Tag,
            waitingTask.Priority,
            waitingTask.Userdata,
            TaskStatus.Todo,
            waitingTask.Description);
        results.Add(newTaskInformation);
      }
    }
  }

  /// <summary>
  /// Retrieves information for all tasks currently managed by the pool.
  /// </summary>
  /// <returns>Task information</returns>
  public TaskInformation[] GetAllTaskInformations() {
    int index = 0;
    TaskInformation[] results = new TaskInformation[_workingAgents.Count + _waitingTasks.Count];
    foreach (ITaskAgent<T> workingAgent in _workingAgents) {
      T workingTask = workingAgent.Task;
      TaskStatus isDone = workingTask.Done ? TaskStatus.Done : TaskStatus.Doing;
      results[index++] = new TaskInformation(workingTask.SerialId,
          workingTask.Tag,
          workingTask.Priority,
          workingTask.Userdata,
          isDone,
          workingTask.Description);
    }

    foreach (T waitingTask in _waitingTasks) {
      results[index++] = new TaskInformation(waitingTask.SerialId,
          waitingTask.Tag,
          waitingTask.Priority,
          waitingTask.Userdata,
          TaskStatus.Todo,
          waitingTask.Description);
    }

    return results;
  }

  /// <summary>
  /// Retrieves information for all tasks currently managed by the pool and stores it in the provided list.
  /// </summary>
  /// <param name="results">Output list for task information</param>
  /// <exception name="ArgumentNullException">Results argument is invalid</exception>
  public void GetAllTaskInformations(List<TaskInformation> results) {
    if (results == null) {
      throw new ArgumentNullException(nameof(results), "Invalid results argument");
    }

    results.Clear();
    foreach (ITaskAgent<T> workingAgent in _workingAgents) {
      T workingTask = workingAgent.Task;
      TaskStatus isDone = workingTask.Done ? TaskStatus.Done : TaskStatus.Doing;
      TaskInformation newTaskInformation = new TaskInformation(workingTask.SerialId,
          workingTask.Tag,
          workingTask.Priority,
          workingTask.Userdata,
          isDone,
          workingTask.Description);
      results.Add(newTaskInformation);
    }

    foreach (T waitingTask in _waitingTasks) {
      TaskInformation newTaskInformation = new TaskInformation(waitingTask.SerialId,
          waitingTask.Tag,
          waitingTask.Priority,
          waitingTask.Userdata,
          TaskStatus.Todo,
          waitingTask.Description);
      results.Add(newTaskInformation);
    }
  }

  /// <summary>
  /// Adds a task to the pool.
  /// </summary>
  /// <param name="task">Task to add to the pool.</param>
  public void AddTask(T task) {
    LinkedListNode<T>? current = _waitingTasks.Last;
    while (current != null) {
      if (task.Priority <= current.Value.Priority) {
        break;
      }

      current = current.Previous!;
    }

    if (current != null) {
      _waitingTasks.AddAfter(current, task);
    } else {
      _waitingTasks.AddFirst(task);
    }
  }

  /// <summary>
  /// Removes a task from the pool based on its serial ID.
  /// </summary>
  /// <param name="serialId">Remove the task with this serial ID</param>
  /// <returns>True if the task was removed successfully; otherwise, false.</returns>
  public bool RemoveTask(int serialId) {
    foreach (T task in _waitingTasks) {
      if (task.SerialId == serialId) {
        _waitingTasks.Remove(task);
        ReferencePool.Release(task);
        return true;
      }
    }

    LinkedListNode<ITaskAgent<T>>? currentWorkingAgent = _workingAgents.First;
    while (currentWorkingAgent != null) {
      LinkedListNode<ITaskAgent<T>> next = currentWorkingAgent.Next!;
      ITaskAgent<T> workingAgent = currentWorkingAgent.Value;
      T task = workingAgent.Task;
      if (task.SerialId == serialId) {
        workingAgent.Reset();
        _freeAgents.Push(workingAgent);
        _workingAgents.Remove(currentWorkingAgent);
        ReferencePool.Release(task);
        return true;
      }

      currentWorkingAgent = next;
    }

    return false;
  }

  /// <summary>
  /// Removes tasks from the pool that match the specified tag label.
  /// </summary>
  /// <param name="tag">Remove tasks with this tag label</param>
  /// <returns>The number of tasks removed from the pool.</returns>
  public int RemoveTasks(string tag) {
    int count = 0;

    LinkedListNode<T>? currentWaitingTask = _waitingTasks.First;
    while (currentWaitingTask != null) {
      LinkedListNode<T>? next = currentWaitingTask.Next;
      T task = currentWaitingTask.Value;
      if (task.Tag == tag) {
        _waitingTasks.Remove(currentWaitingTask);
        ReferencePool.Release(task);
        count++;
      }

      currentWaitingTask = next;
    }

    LinkedListNode<ITaskAgent<T>>? currentWorkingAgent = _workingAgents.First;
    while (currentWorkingAgent != null) {
      LinkedListNode<ITaskAgent<T>>? next = currentWorkingAgent.Next;
      ITaskAgent<T> workingAgent = currentWorkingAgent.Value;
      T task = workingAgent.Task;
      if (task.Tag == tag) {
        workingAgent.Reset();
        _freeAgents.Push(workingAgent);
        _workingAgents.Remove(currentWorkingAgent);
        ReferencePool.Release(task);
        count++;
      }

      currentWorkingAgent = next;
    }

    return count;
  }

  /// <summary>
  /// Removes all tasks from the pool.
  /// </summary>
  /// <returns>The number of tasks removed from the pool.</returns>
  public int RemoveAllTasks() {
    int count = _waitingTasks.Count + _workingAgents.Count;

    foreach (T task in _waitingTasks) {
      ReferencePool.Release(task);
    }

    _waitingTasks.Clear();

    foreach (ITaskAgent<T> workingAgent in _workingAgents) {
      T task = workingAgent.Task;
      workingAgent.Reset();
      _freeAgents.Push(workingAgent);
      ReferencePool.Release(task);
    }

    _workingAgents.Clear();

    return count;
  }

  private void ProcessRunningTasks(float elapseSeconds, float realElapseSeconds) {
    LinkedListNode<ITaskAgent<T>>? current = _workingAgents.First;
    while (current != null) {
      T task = current.Value.Task;
      if (!task.Done) {
        current.Value.Update(elapseSeconds, realElapseSeconds);
        current = current.Next;
        continue;
      }

      LinkedListNode<ITaskAgent<T>>? next = current.Next;
      current.Value.Reset();
      _freeAgents.Push(current.Value);
      _workingAgents.Remove(current);
      ReferencePool.Release(task);
      current = next;
    }
  }

  private void ProcessWaitingTasks(float elapseSeconds, float realElapseSeconds) {
    LinkedListNode<T>? current = _waitingTasks.First;
    while (current != null && FreeAgentCount > 0 && CanProcessNextTask(current.Value)) {
      ITaskAgent<T> agent = _freeAgents.Pop()!;
      LinkedListNode<ITaskAgent<T>> agentNode = _workingAgents.AddLast(agent)!;
      T task = current.Value;
      LinkedListNode<T>? next = current.Next;
      StartTaskStatus status = agent.Start(task);
      if (status == StartTaskStatus.Done ||
          status == StartTaskStatus.HasToWait ||
          status == StartTaskStatus.UnknownError) {
        agent.Reset();
        _freeAgents.Push(agent);
        _workingAgents.Remove(agentNode);
      }

      if (status == StartTaskStatus.Done ||
          status == StartTaskStatus.CanResume ||
          status == StartTaskStatus.UnknownError) {
        _waitingTasks.Remove(current);
      }

      if (status == StartTaskStatus.Done || status == StartTaskStatus.UnknownError) {
        ReferencePool.Release(task);
      }

      current = next;
    }
  }

  // Determines whether the next task in the queue has a higher or equal priority compared to the currently running tasks.
  private bool CanProcessNextTask(TaskBase nextTask) {
    if (!_canProcessNextTaskEnabled)  {
      return true;
    }

    var workingTask = _workingAgents.First;
    if (workingTask == null) {
      return true;
    }

    var waitingTask = _waitingTasks.First;
    if (waitingTask == null) {
      return true;
    }

    if (nextTask.Priority != workingTask.Value.Task.Priority) {
      return false;
    }

    return waitingTask.Value.Priority != workingTask.Value.Task.Priority ? false : true;
  }
}
