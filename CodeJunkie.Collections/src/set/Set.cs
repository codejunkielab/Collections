namespace CodeJunkie.Collections;

using System.Collections.Generic;

/// <summary>
/// Set class extending <see cref="HashSet{T}"/> and implementing <see cref="IReadOnlySet{T}"/>.
/// </summary>
/// <typeparam name="T">Type of elements in the set.</typeparam>
public class Set<T> : HashSet<T>, IReadOnlySet<T>;
