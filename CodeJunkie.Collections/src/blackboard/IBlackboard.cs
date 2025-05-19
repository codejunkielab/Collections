namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Represents a shared data table where data is accessed by its type.
/// This allows data to be shared across different states.
/// </summary>
public interface IBlackboard : IReadOnlyBlackboard {
  /// <summary>
  /// Adds data to the blackboard, retrievable by its compile-time type.
  /// Note: Only one piece of data per type is allowed.
  /// </summary>
  /// <param name="data">The data to store in the blackboard.</param>
  /// <typeparam name="TData">The type of the data to add.</typeparam>
  /// <exception cref="KeyNotFoundException">Thrown if the data type is not found.</exception>
  void Set<TData>(TData data) where TData : class;

  /// <summary>
  /// Adds data to the blackboard and associates it with a specified type.
  /// Note: Only one piece of data per type is allowed.
  /// </summary>
  /// <param name="type">Type of the data.</param>
  /// <param name="data">Data to write to the blackboard.</param>
  /// <exception cref="DuplicateNameException" />
  void SetObject(Type type, object data);

  /// <summary>
  /// Adds or overwrites data in the blackboard, retrievable by its compile-time type.
  /// Unlike <see cref="Set{TData}(TData)" />, this method overwrites existing data of the same type.
  /// </summary>
  /// <param name="data">Data to write to the blackboard.</param>
  /// <typeparam name="TData">Type of the data to add or overwrite.</typeparam>
  void Overwrite<TData>(TData data) where TData : class;

  /// <summary>
  /// Adds or overwrites data in the blackboard and associates it with a specified type.
  /// Unlike <see cref="Set{TData}(TData)" />, this method overwrites existing data of the same type.
  /// </summary>
  /// <param name="type">Type of the data.</param>
  /// <param name="data">Data to write to the blackboard.</param>
  void OverwriteObject(Type type, object data);
}
