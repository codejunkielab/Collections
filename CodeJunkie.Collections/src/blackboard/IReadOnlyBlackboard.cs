namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a read-only blackboard, which is a table of data.
/// Data is accessed by its type and shared across different states.
/// </summary>
public interface IReadOnlyBlackboard {
  /// <summary>
  /// Gets all types currently present in the blackboard.
  /// </summary>
  /// <returns>A read-only set of types available in the blackboard.</returns>
  IReadOnlySet<Type> Types { get; }

  /// <summary>
  /// Retrieves data from the blackboard using its compile-time type.
  /// </summary>
  /// <typeparam name="TData">The type of data to retrieve.</typeparam>
  /// <exception cref="KeyNotFoundException">Thrown if the specified type is not found in the blackboard.</exception>
  TData Get<TData>() where TData : class;

  /// <summary>
  /// Retrieves data from the blackboard using its runtime type.
  /// </summary>
  /// <param name="type">The runtime type of the data to retrieve.</param>
  /// <exception cref="KeyNotFoundException">Thrown if the specified type is not found in the blackboard.</exception>
  object GetObject(Type type);

  /// <summary>
  /// Checks if the blackboard contains data of the specified type.
  /// </summary>
  /// <typeparam name="TData">The type of data to look for.</typeparam>
  /// <returns>True if the blackboard contains data of the specified type; otherwise, false.</returns>
  bool Has<TData>() where TData : class;

  /// <summary>
  /// Checks whether the blackboard contains data of the specified runtime type.
  /// </summary>
  /// <param name="type">The runtime type of the data to check for.</param>
  /// <returns>True if the blackboard contains data of the specified type; otherwise, false.</returns>
  bool HasObject(Type type);
}
