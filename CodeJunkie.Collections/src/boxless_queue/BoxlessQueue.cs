namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// A specialized queue for storing multiple struct types without boxing,
/// reducing heap allocations and improving performance.
/// </summary>
/// <remarks>
/// This queue dynamically creates internal queues for each struct type,
/// leveraging standard queue functionality while minimizing memory churn
/// caused by boxing and unboxing operations.
/// Adapted from https://stackoverflow.com/a/6164880.
/// </remarks>
/// <param name="handler"><inheritdoc cref="Handler" path="/summary"/>
/// </param>
public class BoxlessQueue(IBoxlessValueHandler handler) {
  private abstract class TypedValueQueue {
    public abstract void HandleValue(IBoxlessValueHandler handler);
    public abstract void Clear();
  }

  private class TypedMessageQueue<T> : TypedValueQueue where T : struct {
    private readonly Queue<T> _queue = new();

    public void Enqueue(T message) => _queue.Enqueue(message);

    public override void HandleValue(IBoxlessValueHandler handler) =>
      handler.HandleValue(_queue.Dequeue());

    public override void Clear() => _queue.Clear();
  }

  /// <summary>
  /// The handler that processes dequeued values, avoiding boxing and unboxing.
  /// </summary>
  public IBoxlessValueHandler Handler { get; } = handler;
  private readonly Queue<Type> _queueSelectorQueue = new();
  private readonly Dictionary<Type, TypedValueQueue> _queues = [];

  /// <summary>
  /// Enqueues a value without boxing.
  /// </summary>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  public void Enqueue<TValue>(TValue message) where TValue : struct {
    TypedMessageQueue<TValue> queue;

    if (!_queues.ContainsKey(typeof(TValue))) {
      queue = new TypedMessageQueue<TValue>();
      _queues[typeof(TValue)] = queue;
    }
    else {
      queue = (TypedMessageQueue<TValue>)_queues[typeof(TValue)];
    }

    queue.Enqueue(message);
    _queueSelectorQueue.Enqueue(typeof(TValue));
  }

  /// <summary>
  /// Checks if the queue contains any values.
  /// </summary>
  public bool HasValues => _queueSelectorQueue.Count > 0;

  /// <summary>
  /// Dequeues and processes the next value, if available.
  /// </summary>
  public void Dequeue() {
    if (!HasValues) { return; }
    var type = _queueSelectorQueue.Dequeue();
    _queues[type].HandleValue(Handler);
  }

  /// <summary>
  /// Clears all values from the queue.
  /// </summary>
  public void Clear() {
    _queueSelectorQueue.Clear();

    foreach (var queue in _queues.Values) {
      queue.Clear();
    }
  }
}
