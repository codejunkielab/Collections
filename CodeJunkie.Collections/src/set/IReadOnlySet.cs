namespace CodeJunkie.Collections;

using System.Collections.Generic;

/// <summary>
/// Read-only interface for a set of elements.
/// </summary>
/// <typeparam name="T">Type of elements.</typeparam>
public interface IReadOnlySet<T> : IReadOnlyCollection<T> {

  /// <summary>
  /// Checks if the current set is a subset of the specified collection.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if all elements of this set are in other; otherwise false.</returns>
  /// <remarks>Equal sets count as subsets. Returns false if any element is missing in other.</remarks>
  bool IsSubsetOf(IEnumerable<T> other);

  /// <summary>
  /// Checks if the current set is a superset of the specified collection.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if this set contains all elements of other; otherwise false.</returns>
  /// <remarks>Equal sets count as supersets. Returns false if any element of other is missing.</remarks>
  bool IsSupersetOf(IEnumerable<T> other);

  /// <summary>
  /// Checks if the current set is a proper (strict) superset of the specified collection.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if this set contains all elements of other plus at least one additional element.</returns>
  /// <remarks>
  /// An empty set is a proper superset of an empty collection only if this set is not empty.
  /// Returns false if this set has fewer or equal elements than other.
  /// </remarks>
  bool IsProperSupersetOf(IEnumerable<T> other);

  /// <summary>
  /// Checks if the current set is a proper (strict) subset of the specified collection.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if this set is contained in other and other has at least one additional element.</returns>
  /// <remarks>
  /// An empty set is a proper subset of any non-empty collection.
  /// Returns false if this set has more or equal elements than other.
  /// </remarks>
  bool IsProperSubsetOf(IEnumerable<T> other);

  /// <summary>
  /// Checks if the current set shares any elements with the specified collection.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if there is at least one common element; otherwise false.</returns>
  /// <remarks>Duplicate elements in other are ignored.</remarks>
  bool Overlaps(IEnumerable<T> other);

  /// <summary>
  /// Checks if the current set and specified collection contain the same elements.
  /// </summary>
  /// <param name="other">Collection to compare with.</param>
  /// <returns>True if both sets are equal ignoring order and duplicates; otherwise false.</returns>
  bool SetEquals(IEnumerable<T> other);
}
