namespace CodeJunkie.Collections.Tests;

using Shouldly;
using System;
using System.Collections.Generic;
using CodeJunkie.Collections;
using Xunit;

public sealed class TaskPoolTests {
  private sealed class StubTaskAgent : ITaskAgent<MockTaskBase> {
    /// <summary>
    /// Returns the task
    /// </summary>
    public MockTaskBase Task { get; private set; } = default!;

    /// <summary>
    /// Initializes the task agent
    /// </summary>
    void ITaskAgent<MockTaskBase>.Initialize() { }

    /// <summary>
    /// Polls the task agent
    /// </summary>
    /// <param name="elapseSeconds">Logical elapsed time (in seconds)</param>
    /// <param name="realElapseSeconds">Actual elapsed time (in seconds)</param>
    void ITaskAgent<MockTaskBase>.Update(float elapseSeconds, float realElapseSeconds) { }

    /// <summary>
    /// Shuts down and cleans up the task agent
    /// </summary>
    void ITaskAgent<MockTaskBase>.Shutdown() { }

    /// <summary>
    /// Starts processing the task
    /// </summary>
    /// <param name="task">The task to process</param>
    /// <returns>Returns the status of the task</returns>
    StartTaskStatus ITaskAgent<MockTaskBase>.Start(MockTaskBase task) {
      Task = task;
      return StartTaskStatus.Done;
    }

    /// <summary>
    /// Stops the ongoing task and resets the task agent
    /// </summary>
    void ITaskAgent<MockTaskBase>.Reset() { }
  }

  private sealed class MockTaskBase : TaskBase {
    public override string? Description => "Test Task";
  }

  private readonly StubTaskAgent _stubAgent;
  private readonly TaskPool<MockTaskBase> _taskPool;

  public TaskPoolTests() {
    _stubAgent = new StubTaskAgent();
    _taskPool = new TaskPool<MockTaskBase>();
  }

  [Fact]
  public void AddAgent_ShouldIncreaseFreeAgentCount() {
    // Act
    _taskPool.AddAgent(_stubAgent);

    // Assert
    _taskPool.FreeAgentCount.ShouldBe(1);
  }

  [Fact]
  public void AddTask_ShouldIncreaseWaitingTaskCount() {
    // Arrange
    var task = new MockTaskBase();

    // Act
    _taskPool.AddTask(task);

    // Assert
    _taskPool.WaitingTaskCount.ShouldBe(1);
  }

  [Fact]
  public void RemoveTask_ShouldDecreaseWaitingTaskCount() {
    // Arrange
    var task = new MockTaskBase();
    task.Initialize(
        serialId: 1,
        tag: "Test",
        priority: 1,
        userData: "Data");
    _taskPool.AddTask(task);

    // Act
    var result = _taskPool.RemoveTask(1);

    // Assert
    result.ShouldBeTrue();
    _taskPool.WaitingTaskCount.ShouldBe(0);
  }

  [Fact]
  public void GetTaskInformation_ShouldReturnCorrectInformation() {
    // Arrange
    var task = new MockTaskBase();
    task.Initialize(
        serialId: 1,
        tag: "Test",
        priority: 1,
        userData: "Data");
    _taskPool.AddTask(task);

    // Act
    var taskInfo = _taskPool.GetTaskInformation(1);

    // Assert
    taskInfo.SerialId.ShouldBe(1);
    taskInfo.Tag.ShouldBe("Test");
    taskInfo.Priority.ShouldBe(1);
    taskInfo.Userdata.ShouldBe("Data");
    taskInfo.Description.ShouldBe("Test Task");
  }
}
