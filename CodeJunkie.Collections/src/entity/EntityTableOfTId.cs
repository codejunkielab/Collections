namespace CodeJunkie.Collections;

using System.Collections.Concurrent;

/// <summary>
/// Represents a table of entities identified by a unique key of type <typeparamref name="TId"/>.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for entities. Must be non-nullable.</typeparam>
public class EntityTable<TId> where TId : notnull {
  private readonly ConcurrentDictionary<TId, object> _entities = new();

  /// <summary>
  /// Clears all entities from the table.
  /// </summary>
  public void Clear() => _entities.Clear();

  /// <summary>
  /// Attempts to add an entity to the table.
  /// </summary>
  /// <param name="id">The unique identifier for the entity.</param>
  /// <param name="entity">The entity to add.</param>
  /// <returns><c>true</c> if the entity was added successfully; otherwise, <c>false</c>.</returns>
  public bool TryAdd(TId id, object entity) => _entities.TryAdd(id, entity);

  /// <summary>
  /// Removes an entity from the table by its identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the entity to remove. If <c>null</c>, no action is taken.</param>
  public void Remove(TId? id) {
    if (id is null) { return; }

    _entities.TryRemove(id, out _);
  }

  /// <summary>
  /// Sets or updates an entity in the table with the specified identifier.
  /// </summary>
  /// <param name="id">The unique identifier for the entity.</param>
  /// <param name="entity">The entity to set or update.</param>
  public void Set(TId id, object entity) => _entities[id] = entity;

  /// <summary>
  /// Retrieves an entity of the specified type by its identifier.
  /// </summary>
  /// <typeparam name="TUsage">The expected type of the entity.</typeparam>
  /// <param name="id">The unique identifier of the entity to retrieve. If <c>null</c>, <c>default</c> is returned.</param>
  /// <returns>The entity cast to the specified type, or <c>default</c> if not found or the type does not match.</returns>
  public TUsage? Get<TUsage>(TId? id) where TUsage : class {
    if (id is not null &&
        _entities.TryGetValue(id, out var entity) &&
        entity is TUsage expected) {
      return expected;
    }

    return default;
  }
}
