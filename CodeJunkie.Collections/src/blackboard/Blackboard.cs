namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementation of the IBlackboard interface for managing shared data.
/// </summary>
public class Blackboard : IBlackboard {
  /// <summary>
  /// Internal storage for blackboard data, mapping types to their corresponding objects.
  /// </summary>
  protected readonly Dictionary<Type, object> _blackboard = [];

  /// <summary>
  /// Set of types currently stored in the blackboard.
  /// </summary>
  protected readonly Set<Type> _types = [];

  /// <summary>
  /// Initializes a new instance of the <see cref="Blackboard"/> class.
  /// </summary>
  public Blackboard() { }

#region IReadOnlyBlackboard
  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public TData Get<TData>() where TData : class {
    var type = typeof(TData);
    return (TData)GetBlackboardData(type);
  }

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public object GetObject(Type type) {
    return GetBlackboardData(type);
  }

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Has<TData>() where TData : class {
    return HasObject(typeof(TData));
  }

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool HasObject(Type type) {
    return _blackboard.ContainsKey(type);
  }
#endregion IReadOnlyBlackboard

  /// <inheritdoc />
#region Blackboard
  public IReadOnlySet<Type> Types => _types;

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Set<TData>(TData data) where TData : class {
    var type = typeof(TData);
    _types.Add(type);
    SetBlackboardData(type, data);
  }

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void SetObject(Type type, object data) {
    _types.Add(type);
    SetBlackboardData(type, data);
  }

  /// <inheritdoc />
  public void Overwrite<TData>(TData data) where TData : class {
    var type = typeof(TData);
    _types.Add(type);
    OverwriteBlackboardData(type, data);
  }

  /// <inheritdoc />
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void OverwriteObject(Type type, object data) {
    _types.Add(type);
    OverwriteBlackboardData(type, data);
  }
#endregion Blackboard

    /// <summary>
    /// Retrieves data from the blackboard by its type.
    /// </summary>
    /// <param name="type">The type of data to retrieve.</param>
    /// <returns>The data associated with the specified type.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the type is not found in the blackboard.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual object GetBlackboardData(Type type) {
      return _blackboard.TryGetValue(type, out var data)
        ? data
        : throw new KeyNotFoundException($"Data of type {type} not found in the blackboard.");
    }

    /// <summary>
    /// Adds new data to the blackboard.
    /// </summary>
    /// <param name="type">The type of data to add.</param>
    /// <param name="data">The data to store in the blackboard.</param>
    /// <exception cref="DuplicateNameException">Thrown if data of the specified type already exists.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void SetBlackboardData(Type type, object data) {
      if (!_blackboard.TryAdd(type, data)) {
        throw new DuplicateNameException($"Data of type {type} already exists in the blackboard.");
      }
    }

    /// <summary>
    /// Overwrites existing data in the blackboard.
    /// </summary>
    /// <param name="type">The type of data to overwrite.</param>
    /// <param name="data">The new data to store in the blackboard.</param>
    protected virtual void OverwriteBlackboardData(Type type, object data) {
      _blackboard[type] = data;
    }
}
