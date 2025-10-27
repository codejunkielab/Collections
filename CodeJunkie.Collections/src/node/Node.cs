namespace CodeJunkie.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A hierarchical node class with integrated blackboard functionality,
/// similar to Node system but with shared data capabilities.
/// </summary>
public class Node {
  private readonly List<Node> _children;
  private Node _parent;
  private string _name;
  private readonly IBlackboard _blackboard;

  public Node(string name = "Node") {
    _name = name;
    _children = new List<Node>();
    _parent = null;
    _blackboard = new Blackboard();
  }

#region Properties

  /// <summary>
  /// The name of this node.
  /// </summary>
  public string Name {
    get => _name;
    set => _name = value ?? throw new ArgumentNullException(nameof(value));
  }

  /// <summary>
  /// The parent node of this node, or null if this is a root node.
  /// </summary>
  public Node Parent => _parent;

  /// <summary>
  /// A read-only collection of child nodes.
  /// </summary>
  public IReadOnlyList<Node> Children => _children.AsReadOnly();

  /// <summary>
  /// The number of child nodes.
  /// </summary>
  public int ChildCount => _children.Count;

  /// <summary>
  /// Gets whether this node is a root node (has no parent).
  /// </summary>
  public bool IsRoot => _parent == null;

  /// <summary>
  /// Gets whether this node is a leaf node (has no children).
  /// </summary>
  public bool IsLeaf => _children.Count == 0;

  /// <summary>
  /// The blackboard associated with this node.
  /// Each node has its own independent blackboard instance.
  /// </summary>
  public IBlackboard Blackboard => _blackboard;

#endregion

#region Blackboard Access Methods

  /// <summary>
  /// Gets data from this node's blackboard or searches up the hierarchy.
  /// This method first checks the current node's blackboard, then recursively
  /// searches parent nodes until the data is found or the root is reached.
  /// </summary>
  /// <typeparam name="TData">The type of data to retrieve.</typeparam>
  /// <returns>The data of the specified type.</returns>
  /// <exception cref="KeyNotFoundException">Thrown if the data is not found in this node or any parent.</exception>
  public TData Get<TData>() where TData : class {
    // First try to get from current node's blackboard
    if (_blackboard.Has<TData>()) {
      return _blackboard.Get<TData>();
    }

    // If not found and has parent, search up the hierarchy
    if (_parent != null) {
      return _parent.Get<TData>();
    }

    // If reached root and still not found, throw exception
    throw new KeyNotFoundException($"Data of type {typeof(TData)} not found in node hierarchy starting from '{Name}'.");
  }

  /// <summary>
  /// Gets data from this node's blackboard or searches up the hierarchy using runtime type.
  /// </summary>
  /// <param name="type">The runtime type of the data to retrieve.</param>
  /// <returns>The data of the specified type.</returns>
  /// <exception cref="KeyNotFoundException">Thrown if the data is not found in this node or any parent.</exception>
  public object GetObject(Type type) {
    // First try to get from current node's blackboard
    if (_blackboard.HasObject(type)) {
      return _blackboard.GetObject(type);
    }

    // If not found and has parent, search up the hierarchy
    if (_parent != null) {
      return _parent.GetObject(type);
    }

    // If reached root and still not found, throw exception
    throw new KeyNotFoundException($"Data of type {type} not found in node hierarchy starting from '{Name}'.");
  }

  /// <summary>
  /// Checks if data of the specified type exists in this node's blackboard or any parent's blackboard.
  /// </summary>
  /// <typeparam name="TData">The type of data to check for.</typeparam>
  /// <returns>True if the data exists in the hierarchy; otherwise, false.</returns>
  public bool Has<TData>() where TData : class {
    // Check current node's blackboard
    if (_blackboard.Has<TData>()) {
      return true;
    }

    // If not found and has parent, search up the hierarchy
    return _parent?.Has<TData>() ?? false;
  }

  /// <summary>
  /// Checks if data of the specified type exists in this node's blackboard or any parent's blackboard.
  /// </summary>
  /// <param name="type">The runtime type of the data to check for.</param>
  /// <returns>True if the data exists in the hierarchy; otherwise, false.</returns>
  public bool HasObject(Type type) {
    // Check current node's blackboard
    if (_blackboard.HasObject(type)) {
      return true;
    }

    // If not found and has parent, search up the hierarchy
    return _parent?.HasObject(type) ?? false;
  }

  /// <summary>
  /// Sets data in this node's blackboard only (does not affect parent blackboards).
  /// </summary>
  /// <typeparam name="TData">The type of data to set.</typeparam>
  /// <param name="data">The data to store.</param>
  public void Set<TData>(TData data) where TData : class {
    _blackboard.Set(data);
  }

  /// <summary>
  /// Sets data in this node's blackboard using runtime type.
  /// </summary>
  /// <param name="type">The type of the data.</param>
  /// <param name="data">The data to store.</param>
  public void SetObject(Type type, object data) {
    _blackboard.SetObject(type, data);
  }

  /// <summary>
  /// Overwrites data in this node's blackboard.
  /// </summary>
  /// <typeparam name="TData">The type of data to overwrite.</typeparam>
  /// <param name="data">The new data to store.</param>
  public void Overwrite<TData>(TData data) where TData : class {
    _blackboard.Overwrite(data);
  }

  /// <summary>
  /// Overwrites data in this node's blackboard using runtime type.
  /// </summary>
  /// <param name="type">The type of the data.</param>
  /// <param name="data">The new data to store.</param>
  public void OverwriteObject(Type type, object data) {
    _blackboard.OverwriteObject(type, data);
  }

#endregion

#region Child Management

  /// <summary>
  /// Adds a child node to this node.
  /// </summary>
  /// <param name="child">The child node to add.</param>
  /// <exception cref="ArgumentNullException">Thrown if child is null.</exception>
  /// <exception cref="InvalidOperationException">Thrown if trying to add self or create a cycle.</exception>
  public void AddChild(Node child) {
    if (child == null)
      throw new ArgumentNullException(nameof(child));

    if (child == this)
      throw new InvalidOperationException("Cannot add self as child");

    // Check if adding this child would create a cycle
    // This happens if the child is an ancestor of this node
    if (child.IsAncestorOf(this))
      throw new InvalidOperationException("Cannot add ancestor as child (would create cycle)");

    // Remove from previous parent if any
    child._parent?.RemoveChild(child);

    _children.Add(child);
    child._parent = this;

    // Notify about hierarchy changes
    OnChildAdded(child);
    child.OnParentChanged(this);
  }

  /// <summary>
  /// Removes a child node from this node.
  /// </summary>
  /// <param name="child">The child node to remove.</param>
  /// <returns>True if the child was successfully removed; otherwise, false.</returns>
  public bool RemoveChild(Node child) {
    if (child == null || child._parent != this)
      return false;

    bool removed = _children.Remove(child);
    if (removed) {
      child._parent = null;
      OnChildRemoved(child);
      child.OnParentChanged(null);
    }

    return removed;
  }

  /// <summary>
  /// Gets a child node by its index.
  /// </summary>
  /// <param name="index">The index of the child node.</param>
  /// <returns>The child node at the specified index.</returns>
  /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of range.</exception>
  public Node GetChild(int index) {
    if (index < 0 || index >= _children.Count)
      throw new ArgumentOutOfRangeException(nameof(index));

    return _children[index];
  }

  /// <summary>
  /// Gets a child node by its name.
  /// </summary>
  /// <param name="name">The name of the child node to find.</param>
  /// <returns>The first child node with the specified name, or null if not found.</returns>
  public Node GetChild(string name) {
    return _children.FirstOrDefault(child => child.Name == name);
  }

  /// <summary>
  /// Gets the index of a child node.
  /// </summary>
  /// <param name="child">The child node to find the index of.</param>
  /// <returns>The index of the child node, or -1 if not found.</returns>
  public int GetChildIndex(Node child) {
    return _children.IndexOf(child);
  }

  /// <summary>
  /// Removes all child nodes from this node.
  /// </summary>
  public void RemoveAllChildren() {
    var childrenCopy = new List<Node>(_children);
    foreach (var child in childrenCopy) {
      RemoveChild(child);
    }
  }

#endregion

#region Hierarchy Navigation

  /// <summary>
  /// Gets the root node of the hierarchy.
  /// </summary>
  /// <returns>The root node.</returns>
  public Node GetRoot() {
    Node current = this;
    while (current._parent != null) {
      current = current._parent;
    }
    return current;
  }

  /// <summary>
  /// Determines whether this node is an ancestor of the specified node.
  /// </summary>
  /// <param name="node">The node to check.</param>
  /// <returns>True if this node is an ancestor of the specified node; otherwise, false.</returns>
  public bool IsAncestorOf(Node node) {
    if (node == null) return false;

    Node current = node._parent;
    while (current != null) {
      if (current == this)
        return true;
      current = current._parent;
    }
    return false;
  }

  /// <summary>
  /// Determines whether this node is a descendant of the specified node.
  /// </summary>
  /// <param name="node">The node to check.</param>
  /// <returns>True if this node is a descendant of the specified node; otherwise, false.</returns>
  public bool IsDescendantOf(Node node) {
    return node?.IsAncestorOf(this) ?? false;
  }

  /// <summary>
  /// Gets a node by its path relative to this node (e.g., "Child1/Child2").
  /// </summary>
  /// <param name="path">The path to the node.</param>
  /// <returns>The node at the specified path, or null if not found.</returns>
  public Node GetNodeByPath(string path) {
    if (string.IsNullOrEmpty(path))
      return this;

    var parts = path.Split('/');
    Node current = this;

    foreach (var part in parts) {
      if (string.IsNullOrEmpty(part))
        continue;

      current = current.GetChild(part);
      if (current == null)
        return null;
    }

    return current;
  }

  /// <summary>
  /// Gets the path from the root to this node.
  /// </summary>
  /// <returns>The path string from root to this node.</returns>
  public string GetPath() {
    if (_parent == null)
      return Name;

    var path = new List<string>();
    Node current = this;

    while (current != null) {
      path.Insert(0, current.Name);
      current = current._parent;
    }

    return string.Join("/", path);
  }

#endregion

#region Tree Traversal

  /// <summary>
  /// Enumerates all descendant nodes using depth-first traversal.
  /// </summary>
  /// <returns>An enumerable of all descendant nodes.</returns>
  public IEnumerable<Node> GetAllDescendants() {
    foreach (var child in _children) {
      yield return child;
      foreach (var descendant in child.GetAllDescendants()) {
        yield return descendant;
      }
    }
  }

  /// <summary>
  /// Enumerates all descendant nodes using breadth-first traversal.
  /// </summary>
  /// <returns>An enumerable of all descendant nodes in breadth-first order.</returns>
  public IEnumerable<Node> GetAllDescendantsBreadthFirst() {
    var queue = new Queue<Node>(_children);

    while (queue.Count > 0) {
      var current = queue.Dequeue();
      yield return current;

      foreach (var child in current._children) {
        queue.Enqueue(child);
      }
    }
  }

  /// <summary>
  /// Finds the first descendant node that matches the specified predicate.
  /// </summary>
  /// <param name="predicate">The condition to test each node against.</param>
  /// <returns>The first matching descendant node, or null if none found.</returns>
  public Node FindChild(Predicate<Node> predicate) {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));

    foreach (var child in _children) {
      if (predicate(child))
        return child;

      var found = child.FindChild(predicate);
      if (found != null)
        return found;
    }

    return null;
  }

  /// <summary>
  /// Finds all descendant nodes that match the specified predicate.
  /// </summary>
  /// <param name="predicate">The condition to test each node against.</param>
  /// <returns>An enumerable of all matching descendant nodes.</returns>
  public IEnumerable<Node> FindChildren(Predicate<Node> predicate) {
    if (predicate == null)
      throw new ArgumentNullException(nameof(predicate));

    return GetAllDescendants().Where(node => predicate(node));
  }

#endregion

#region Virtual Methods (Override support)

  /// <summary>
  /// Called when a child node is added to this node.
  /// Override this method to implement custom behavior.
  /// </summary>
  /// <param name="child">The child node that was added.</param>
  protected virtual void OnChildAdded(Node child) {
    // Override to implement custom logic
  }

  /// <summary>
  /// Called when a child node is removed from this node.
  /// Override this method to implement custom behavior.
  /// </summary>
  /// <param name="child">The child node that was removed.</param>
  protected virtual void OnChildRemoved(Node child) {
    // Override to implement custom logic
  }

  /// <summary>
  /// Called when the parent of this node changes.
  /// Override this method to implement custom behavior.
  /// </summary>
  /// <param name="newParent">The new parent node, or null if becoming root.</param>
  protected virtual void OnParentChanged(Node newParent) {
    // Override to implement custom logic
  }

#endregion

#region Utility Methods

  /// <summary>
  /// Returns a string representation of the tree structure starting from this node.
  /// </summary>
  /// <param name="indent">The current indentation level.</param>
  /// <returns>A formatted string showing the tree structure.</returns>
  public string PrintTree(int indent = 0) {
    var result = new string(' ', indent * 2) + Name + "\n";

    foreach (var child in _children) {
      result += child.PrintTree(indent + 1);
    }

    return result;
  }

  /// <summary>
  /// Gets the depth of this node in the hierarchy (root = 0).
  /// </summary>
  /// <returns>The depth of this node.</returns>
  public int GetDepth() {
    int depth = 0;
    Node current = _parent;

    while (current != null) {
      depth++;
      current = current._parent;
    }

    return depth;
  }

#endregion

  public override string ToString() {
    return $"Node({Name})";
  }
}
