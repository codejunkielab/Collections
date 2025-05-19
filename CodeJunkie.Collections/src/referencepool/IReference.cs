namespace CodeJunkie.Collections;

/// <summary>
/// Provides a mechanism for managing reusable reference objects.
/// Classes implementing this interface should define how to reset their state
/// to be reused efficiently within a reference pool.
/// </summary>
public interface IReference {
  /// <summary>
  /// Resets the state of the object, preparing it for reuse.
  /// This method should clear any data or state that was set during the object's use.
  /// </summary>
  void Clear();
}
