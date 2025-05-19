namespace CodeJunkie.Collections;

using System.Collections.Generic;

/// <summary>
/// <see cref="HashSet{T}" /> extensions.
/// </summary>
public static class HashSetExtensions {
  /// <summary>
  /// Returns a new hash set that includes the specified item.
  /// </summary>
  /// <typeparam name="T">The type of elements in the hash set.</typeparam>
  /// <param name="set">The original hash set.</param>
  /// <param name="item">The item to add to the new hash set.</param>
  /// <returns>A new hash set containing all elements of the original set and the specified item.</returns>
  public static HashSet<T> With<T>(this HashSet<T> set, T item) {
    var copy = new HashSet<T>(set) { item };
    return copy;
  }

  /// <summary>
  /// Returns a new hash set that excludes the specified item.
  /// </summary>
  /// <typeparam name="T">The type of elements in the hash set.</typeparam>
  /// <param name="set">The original hash set.</param>
  /// <param name="item">The item to remove from the new hash set.</param>
  /// <returns>A new hash set containing all elements of the original set except the specified item.</returns>
  public static HashSet<T> Without<T>(this HashSet<T> set, T item) {
    var copy = new HashSet<T>(set);
    copy.Remove(item);
    return copy;
  }
}
