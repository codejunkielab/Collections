namespace CodeJunkie.Collections;

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// LinkedList class.
/// This class is a wrapper around the System.Collections.Generic.LinkedList class.
/// It provides additional functionality such as caching nodes.
/// </summary>
public sealed class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable {
  /// <summary>
  /// Enumerator class for the LinkedList.
  /// </summary>
  [StructLayout(LayoutKind.Auto)]
  public struct Enumerator : IEnumerator<T>, IEnumerator {
    private System.Collections.Generic.LinkedList<T>.Enumerator _enumerator;

    internal Enumerator(System.Collections.Generic.LinkedList<T> linkedList) {
      if (linkedList == null)
        throw new ArgumentNullException(nameof(linkedList));

      _enumerator = linkedList.GetEnumerator();
    }

    /// <summary>
    /// Returns the current node
    /// </summary>
    public T Current => _enumerator.Current;

    /// <summary>
    /// Returns the current Enumerator
    /// </summary>
    object IEnumerator.Current => _enumerator.Current!;

    /// <summary>
    /// Disposes the Enumerator
    /// </summary>
    public void Dispose() {
      _enumerator.Dispose();
    }

    /// <summary>
    /// Moves to the next node
    /// </summary>
    /// <returns>
    /// Indicates whether the move was successful:
    /// false - Move failed
    /// true - Move succeeded
    /// </returns>
    public bool MoveNext() {
      return _enumerator.MoveNext();
    }

    /// <summary>
    /// Resets the Enumerator counter
    /// </summary>
    void IEnumerator.Reset() {
      ((IEnumerator<T>)_enumerator).Reset();
    }
  }

  private readonly System.Collections.Generic.LinkedList<T> _linkedList = null!;
  private readonly Queue<LinkedListNode<T>> _cachedNodes = null!;

  /// <summary>
  /// Initializes an instance of the LinkedList
  /// </summary>
  public LinkedList() {
    _linkedList = new System.Collections.Generic.LinkedList<T>();
    _cachedNodes = new Queue<LinkedListNode<T>>();
  }

  /// <summary>
  /// Initializes an instance of the LinkedList with a predefined maximum capacity
  /// </summary>
  /// <param name="capacity">The maximum number of elements to manage</param>
  public LinkedList(int capacity) {
    _linkedList = new System.Collections.Generic.LinkedList<T>();
    _cachedNodes = new Queue<LinkedListNode<T>>(capacity);
  }

  /// <summary>
  /// Returns the number of nodes currently in the LinkedList
  /// </summary>
  public int Count => _linkedList.Count;

  /// <summary>
  /// Returns the number of cached nodes in the LinkedList
  /// </summary>
  public int CachedNodeCount => _cachedNodes.Count;

  /// <summary>
  /// Returns the first node in the LinkedList
  /// </summary>
  public LinkedListNode<T>? First => _linkedList.First;

  /// <summary>
  /// Returns the last node in the LinkedList
  /// </summary>
  public LinkedListNode<T>? Last => _linkedList.Last;

  /// <summary>
  /// Returns whether the ICollection is read-only
  /// </summary>
  /// <returns>
  /// Indicates read-only status:
  /// false - Not read-only
  /// true - Read-only
  /// </returns>
  public bool IsReadOnly => ((ICollection<T>)_linkedList).IsReadOnly;

  /// <summary>
  /// Returns an object to synchronize access to the ICollection
  /// </summary>
  public object SyncRoot => ((ICollection)_linkedList).SyncRoot;

  /// <summary>
  /// Returns whether access to the ICollection is thread-safe
  /// </summary>
  /// <returns>
  /// Indicates thread-safety status:
  /// false - Not thread-safe
  /// true - Thread-safe
  /// </returns>
  public bool IsSynchronized => ((ICollection)_linkedList).IsSynchronized;

  /// <summary>
  /// Returns the capacity of the LinkedList
  /// </summary>
  /// <param name="node">The node to check</param>
  /// <param name="value">The value to check</param>
  /// <returns>
  /// Indicates whether the node is valid:
  /// false - Invalid node
  /// true - Valid node
  /// </returns>
  public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value) {
    LinkedListNode<T> newNode = AcquireNode(value);
    _linkedList.AddAfter(node, newNode);
    return newNode;
  }

  /// <summary>
  /// Adds a new node after the specified existing node
  /// </summary>
  /// <param name="node">The existing node</param>
  /// <param name="newNode">The new node to add</param>
  public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode) {
    _linkedList.AddAfter(node, newNode);
  }

  /// <summary>
  /// Adds a new node before the specified existing node
  /// </summary>
  /// <param name="node">The existing node</param>
  /// <param name="value">The value to add</param>
  /// <returns>
  /// The new node containing the specified value
  /// </returns>
  public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value) {
    LinkedListNode<T> newNode = AcquireNode(value);
    _linkedList.AddBefore(node, newNode);
    return newNode;
  }

  /// <summary>
  /// Adds a new node before the specified existing node
  /// </summary>
  /// <param name="node">The existing node</param>
  /// <param name="newNode">The new node to add</param>
  public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode) {
    _linkedList.AddBefore(node, newNode);
  }

  /// <summary>
  /// Adds a new node to the beginning of the LinkedList
  /// </summary>
  /// <param name="value">The value to add</param>
  /// <returns>
  /// The new node containing the specified value
  /// </returns>
  public LinkedListNode<T> AddFirst(T value) {
    LinkedListNode<T> node = AcquireNode(value);
    _linkedList.AddFirst(node);
    return node;
  }

  /// <summary>
  /// Adds a new node to the beginning of the LinkedList
  /// </summary>
  /// <param name="node">The new node to add</param>
  public void AddFirst(LinkedListNode<T> node) {
    _linkedList.AddFirst(node);
  }

  /// <summary>
  /// Adds a new node to the end of the LinkedList
  /// </summary>
  /// <param name="value">The value to add</param>
  /// <returns>
  /// The new node containing the specified value
  /// </returns>
  public LinkedListNode<T> AddLast(T value) {
    LinkedListNode<T> node = AcquireNode(value);
    _linkedList.AddLast(node);
    return node;
  }

  /// <summary>
  /// Adds a new node to the end of the LinkedList
  /// </summary>
  /// <param name="node">The new node to add</param>
  public void AddLast(LinkedListNode<T> node) {
    _linkedList.AddLast(node);
  }

  /// <summary>
  /// Removes all nodes from the LinkedList
  /// </summary>
  public void Clear() {
    LinkedListNode<T>? current = _linkedList.First;
    while (current != null) {
      ReleaseNode(current);
      current = current.Next!;
    }

    _linkedList.Clear();
  }

  /// <summary>
  /// Clears the cached nodes
  /// </summary>
  public void ClearCachedNodes() {
    _cachedNodes.Clear();
  }

  /// <summary>
  /// Checks if the LinkedList contains a specific item
  /// </summary>
  /// <param name="item">The item to check</param>
  /// <returns>
  /// Indicates whether the item was found:
  /// false - Not found
  /// true - Found
  /// </returns>
  public bool Contains(T item) {
    return _linkedList.Contains(item);
  }

  /// <summary>
  /// Checks if the LinkedList contains a specific node
  /// </summary>
  /// <param name="array">The array to check</param>
  /// <param name="index">The index of the node</param>
  public void CopyTo(T[] array, int index) {
    _linkedList.CopyTo(array, index);
  }

  /// <summary>
  /// Copies the contents of the LinkedList to an array
  /// </summary>
  /// <param name="array">The array to copy to</param>
  /// <param name="index">The starting index in the array</param>
  public void CopyTo(Array array, int index) {
    ((ICollection)_linkedList).CopyTo(array, index);
  }

  /// <summary>
  /// Finds the first node containing the specified value
  /// </summary>
  /// <param name="value">The value to find</param>
  /// <returns>
  /// The node containing the specified value
  /// If not found, null is returned
  /// </returns>
  public LinkedListNode<T>? Find(T value) {
    return _linkedList.Find(value);
  }

  /// <summary>
  /// Finds the last node containing the specified value
  /// </summary>
  /// <param name="value">The value to find</param>
  /// <returns>
  /// The node containing the specified value
  /// If not found, null is returned
  /// </returns>
  public LinkedListNode<T>? FindLast(T value) {
    return _linkedList.FindLast(value);
  }

  /// <summary>
  /// Removes a specific item from the LinkedList
  /// </summary>
  /// <param name="item">The item to remove</param>
  /// <returns>
  /// Indicates whether the removal was successful:
  /// false - Not successful
  /// true - Successful
  /// </returns>
  public bool Remove(T item) {
    var node = _linkedList.Find(item);
    if (node != null) {
      _linkedList.Remove(node);
      ReleaseNode(node);
      return true;
    }

    return false;
  }

  /// <summary>
  /// Removes a specific node from the LinkedList
  /// </summary>
  /// <param name="node">The node to remove</param>
  public void Remove(LinkedListNode<T> node) {
    _linkedList.Remove(node);
    ReleaseNode(node);
  }

  /// <summary>
  /// Removes the first node from the LinkedList
  /// </summary>
  public void RemoveFirst() {
    var first = _linkedList.First;
    if (first == null) {
      throw new InvalidOperationException("Invalid first node.");
    }

    _linkedList.RemoveFirst();
    ReleaseNode(first);
  }

  /// <summary>
  /// Removes the last node from the LinkedList
  /// </summary>
  public void RemoveLast() {
    var last = _linkedList.Last;
    if (last == null) {
      throw new InvalidOperationException("Invalid last node.");
    }

    _linkedList.RemoveLast();
    ReleaseNode(last);
  }

  /// <summary>
  /// Returns an enumerator for the LinkedList
  /// </summary>
  /// <returns>
  /// An enumerator for the LinkedList
  /// This method allocates memory, so be careful
  /// </returns>
  public Enumerator GetEnumerator() {
    return new Enumerator(_linkedList);
  }

  private LinkedListNode<T> AcquireNode(T value) {
    var node = default(LinkedListNode<T>);
    if (_cachedNodes.Count > 0) {
      node = _cachedNodes.Dequeue();
      node.Value = value;
    } else {
      node = new LinkedListNode<T>(value);
    }

    return node;
  }

  private void ReleaseNode(LinkedListNode<T> node) {
#pragma warning disable CS8601
    node.Value = default(T);
#pragma warning restore CS8601
    _cachedNodes.Enqueue(node);
  }

  /// <inheritdoc/>
  void ICollection<T>.Add(T value) {
    AddLast(value);
  }

  /// <inheritdoc/>
  IEnumerator<T> IEnumerable<T>.GetEnumerator() {
    return GetEnumerator();
  }

  /// <inheritdoc/>
  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}
