namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides a mechanism for managing object pooling to reduce memory allocation and improve performance.
/// This class allows acquiring, releasing, adding, and removing references of specific types.
/// </summary>
public static class ReferencePool {
  private sealed class ReferenceCollection {
    private readonly Queue<IReference> _references;
    private readonly Type _referenceType;
    private int _usingReferenceCount;
    private int _acquireReferenceCount;
    private int _releaseReferenceCount;
    private int _addReferenceCount;
    private int _removeReferenceCount;

    /// <summary>
    /// Gets the type of the reference managed by this collection.
    /// </summary>
    public Type ReferenceType => _referenceType;

    /// <summary>
    /// Gets the count of references that are currently unused and available for reuse.
    /// </summary>
    public int UnusedReferenceCount => _references.Count;

    /// <summary>
    /// Gets the count of references that are currently in use.
    /// </summary>
    public int UsingReferenceCount => _usingReferenceCount;

    /// <summary>
    /// Gets the total count of references that have been acquired from the pool.
    /// </summary>
    public int AcquireReferenceCount => _acquireReferenceCount;

    /// <summary>
    /// Gets the total count of references that have been released back to the pool.
    /// </summary>
    public int ReleaseReferenceCount => _releaseReferenceCount;

    /// <summary>
    /// Gets the total count of references that have been added to the pool.
    /// </summary>
    public int AddReferenceCount => _addReferenceCount;

    /// <summary>
    /// Gets the total count of references that have been removed from the pool.
    /// </summary>
    public int RemoveReferenceCount => _removeReferenceCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceCollection"/> class
    /// to manage references of a specific type.
    /// </summary>
    /// <param name="referenceType">
    /// The type of references to be managed by this collection.
    /// </param>
    public ReferenceCollection(Type referenceType) {
      _references = new Queue<IReference>();
      _referenceType = referenceType;
      _usingReferenceCount = 0;
      _acquireReferenceCount = 0;
      _releaseReferenceCount = 0;
      _addReferenceCount = 0;
      _removeReferenceCount = 0;
    }

    /// <summary>
    /// Acquires a reference of the specified type from the pool.
    /// If no references are available, a new one is created.
    /// </summary>
    /// <typeparam name="T">The type of reference to acquire.</typeparam>
    /// <returns>An instance of the specified reference type.</returns>
    /// <exception>
    /// Thrown when the type being acquired does not match the type managed by this collection.
    /// </exception>
    public T Acquire<T>() where T : class, IReference, new() {
      if (typeof(T) != _referenceType) {
        throw new InvalidOperationException("The type being acquired does not match the type managed by this collection.");
      }

      _usingReferenceCount++;
      _acquireReferenceCount++;
      lock (_references) {
        if (_references.Count > 0) {
          return (T)_references.Dequeue();
        }
      }

      _addReferenceCount++;
      return new T();
    }

    /// <summary>
    /// Acquires a reference from the pool.
    /// If no references are available, a new one is created.
    /// </summary>
    /// <returns>An instance of the reference.</returns>
    public IReference Acquire() {
      _usingReferenceCount++;
      _acquireReferenceCount++;
      lock (_references) {
        if (_references.Count > 0) {
          return _references.Dequeue();
        }
      }

      _addReferenceCount++;
      return (IReference)Activator.CreateInstance(_referenceType!)!;
    }

    /// <summary>
    /// Releases a reference back to the pool after clearing its state.
    /// </summary>
    /// <param name="reference">The reference to release.</param>
    /// <exception>
    /// Thrown if the reference has already been released and strict checking is enabled.
    /// </exception>
    public void Release(IReference reference) {
      reference.Clear();
      lock (_references) {
        if (_enableStrictCheck && _references.Contains(reference)) {
          throw new InvalidOperationException("The reference has already been released.");
        }

        _references.Enqueue(reference);
      }

      ++_releaseReferenceCount;
      --_usingReferenceCount;
    }

    /// <summary>
    /// Adds a specified number of references of the given type to the pool.
    /// </summary>
    /// <typeparam name="T">The type of reference to add.</typeparam>
    /// <param name="count">The number of references to add.</param>
    public void Add<T>(int count) where T : class, IReference, new() {
      if (typeof(T) != _referenceType) {
        throw new InvalidOperationException("The type being added does not match the type managed by this collection.");
      }

      lock (_references) {
        _addReferenceCount += count;
        while (count-- > 0) {
          _references.Enqueue(new T());
        }
      }
    }

    /// <summary>
    /// Adds a specified number of references to the pool.
    /// </summary>
    /// <param name="count">The number of references to add.</param>
    public void Add(int count) {
      lock (_references) {
        _addReferenceCount += count;
        while (count-- > 0) {
          _references.Enqueue((IReference)Activator.CreateInstance(_referenceType!)!);
        }
      }
    }

    /// <summary>
    /// Removes a specified number of references from the pool.
    /// </summary>
    /// <param name="count">The number of references to remove.</param>
    public void Remove(int count) {
      lock (_references) {
        if (count > _references.Count) {
          count = _references.Count;
        }

        _removeReferenceCount += count;
        while (count-- > 0) {
          _references.Dequeue();
        }
      }
    }

    /// <summary>
    /// Removes all references from the pool.
    /// </summary>
    public void RemoveAll() {
      lock (_references) {
        _removeReferenceCount += _references.Count;
        _references.Clear();
      }
    }
  }

  private static readonly Dictionary<Type, ReferenceCollection> _referenceCollections = new Dictionary<Type, ReferenceCollection>();
  private static bool _enableStrictCheck;

  /// <summary>
  /// Accessor and mutator for enabling or disabling strict checks.
  /// </summary>
  public static bool EnableStrictCheck {
    get => _enableStrictCheck;
    set => _enableStrictCheck = value;
  }

  /// <summary>
  /// Accessor for the number of reference pools.
  /// </summary>
  public static int Count => _referenceCollections.Count;

  /// <summary>
  /// Returns information about all reference pools.
  /// </summary>
  /// <returns>Array of reference pool information.</returns>
  public static ReferencePoolInformation[] GetAllReferencePoolInfos() {
    var index = 0;
    ReferencePoolInformation[] results = null!;

    lock (_referenceCollections) {
      results = new ReferencePoolInformation[_referenceCollections.Count];
      foreach (KeyValuePair<Type, ReferenceCollection> referenceCollection in _referenceCollections) {
        results[index++] = new ReferencePoolInformation(referenceCollection.Key,
            referenceCollection.Value.UnusedReferenceCount,
            referenceCollection.Value.UsingReferenceCount,
            referenceCollection.Value.AcquireReferenceCount,
            referenceCollection.Value.ReleaseReferenceCount,
            referenceCollection.Value.AddReferenceCount,
            referenceCollection.Value.RemoveReferenceCount);
      }
    }

    return results;
  }

  /// <summary>
  /// Clears all reference pools.
  /// </summary>
  public static void ClearAll() {
    lock (_referenceCollections) {
      foreach (KeyValuePair<Type, ReferenceCollection> referenceCollection in _referenceCollections) {
        referenceCollection.Value.RemoveAll();
      }

      _referenceCollections.Clear();
    }
  }

  /// <summary>
  /// Acquires a reference from the reference pool.
  /// </summary>
  /// <typeparam name="T">Type of the reference.</typeparam>
  /// <returns>Reference of type T.</returns>
  public static T Acquire<T>() where T : class, IReference, new() {
    return GetReferenceCollection(typeof(T)).Acquire<T>();
  }

  /// <summary>
  /// Acquires a reference of the specified type from the reference pool.
  /// </summary>
  /// <param name="referenceType">Type of the reference.</param>
  /// <returns>Reference of the specified type.</returns>
  public static IReference Acquire(Type referenceType) {
    InternalCheckReferenceType(referenceType);
    return GetReferenceCollection(referenceType).Acquire();
  }

  /// <summary>
  /// Releases a reference back to the reference pool.
  /// </summary>
  /// <param name="reference">The reference to release.</param>
  /// <exception name="FrameworkException">
  /// Thrown when the reference argument is invalid.
  /// </exception>
  public static void Release(IReference reference) {
    if (reference == null) {
      throw new ArgumentNullException(nameof(reference), "The reference argument cannot be null.");
    }

    Type referenceType = reference.GetType();
    InternalCheckReferenceType(referenceType);
    GetReferenceCollection(referenceType).Release(reference);
  }

  /// <summary>
  /// Adds the specified number of references to the reference pool.
  /// </summary>
  /// <typeparam name="T">Type of the reference.</typeparam>
  /// <param name="count">Number of references to add.</param>
  public static void Add<T>(int count) where T : class, IReference, new() {
    GetReferenceCollection(typeof(T)).Add<T>(count);
  }

  /// <summary>
  /// Adds the specified number of references of the given type to the reference pool.
  /// </summary>
  /// <param name="referenceType">Type of the reference.</param>
  /// <param name="count">Number of references to add.</param>
  public static void Add(Type referenceType, int count) {
    InternalCheckReferenceType(referenceType);
    GetReferenceCollection(referenceType).Add(count);
  }

  /// <summary>
  /// Removes the specified number of references from the reference pool.
  /// </summary>
  /// <typeparam name="T">Type of the reference.</typeparam>
  /// <param name="count">Number of references to remove.</param>
  public static void Remove<T>(int count) where T : class, IReference {
    GetReferenceCollection(typeof(T)).Remove(count);
  }

  /// <summary>
  /// Removes the specified number of references of the given type from the reference pool.
  /// </summary>
  /// <param name="referenceType">Type of the reference.</param>
  /// <param name="count">Number of references to remove.</param>
  public static void Remove(Type referenceType, int count) {
    InternalCheckReferenceType(referenceType);
    GetReferenceCollection(referenceType).Remove(count);
  }

  /// <summary>
  /// Removes all references of the specified type from the reference pool.
  /// </summary>
  /// <typeparam name="T">Type of the reference.</typeparam>
  public static void RemoveAll<T>() where T : class, IReference {
    GetReferenceCollection(typeof(T)).RemoveAll();
  }

  /// <summary>
  /// Removes all references of the given type from the reference pool.
  /// </summary>
  /// <param name="referenceType">Type of the reference.</param>
  public static void RemoveAll(Type referenceType) {
    InternalCheckReferenceType(referenceType);
    GetReferenceCollection(referenceType).RemoveAll();
  }

  private static void InternalCheckReferenceType(Type referenceType) {
    if (!_enableStrictCheck) {
      return;
    }

    if (referenceType == null) {
      throw new ArgumentNullException(nameof(referenceType), "The reference type argument cannot be null.");
    }

    if (!referenceType.IsClass || referenceType.IsAbstract) {
      throw new ArgumentException("The reference type must be a non-abstract class type.", nameof(referenceType));
    }

    if (!typeof(IReference).IsAssignableFrom(referenceType)) {
      throw new InvalidOperationException($"The reference type '{referenceType.FullName}' is not valid.");
    }
  }

  private static ReferenceCollection GetReferenceCollection(Type referenceType) {
    if (referenceType == null) {
      throw new ArgumentNullException(nameof(referenceType), "The reference type argument cannot be null.");
    }

    ReferenceCollection referenceCollection = null!;
    lock (_referenceCollections) {
      if (!_referenceCollections.TryGetValue(referenceType!, out referenceCollection!)) {
        referenceCollection = new ReferenceCollection(referenceType);
        _referenceCollections.Add(referenceType, referenceCollection);
      }
    }

    return referenceCollection;
  }
}
