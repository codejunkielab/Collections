namespace CodeJunkie.Collections;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Represents information about the reference pool.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct ReferencePoolInformation {
  /// <summary>
  /// Gets the type of the reference pool.
  /// </summary>
  public Type Type { get; private set; }

  /// <summary>
  /// Gets the number of unused references in the pool.
  /// </summary>
  public int UnusedReferenceCount { get; private set; }

  /// <summary>
  /// Gets the number of references currently in use.
  /// </summary>
  public int UsingReferenceCount { get; private set; }

  /// <summary>
  /// Gets the total number of references acquired from the pool.
  /// </summary>
  public int AcquireReferenceCount { get; private set; }

  /// <summary>
  /// Gets the total number of references released back to the pool.
  /// </summary>
  public int ReleaseReferenceCount { get; private set; }

  /// <summary>
  /// 追加可能なリファレンス数へのアクセサ
  /// </summary>
  public int AddReferenceCount { get; private set; }

  /// <summary>
  /// Gets the total number of references removed from the pool.
  /// </summary>
  public int RemoveReferenceCount { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ReferencePoolInformation"/> struct with specified parameters.
  /// </summary>
  /// <param name="type">The type of the reference pool.</param>
  /// <param name="unusedReferenceCount">The number of unused references in the pool.</param>
  /// <param name="usingReferenceCount">The number of references currently in use.</param>
  /// <param name="acquireReferenceCount">The total number of references acquired from the pool.</param>
  /// <param name="releaseReferenceCount">The total number of references released back to the pool.</param>
  /// <param name="addReferenceCount">The total number of references added to the pool.</param>
  /// <param name="removeReferenceCount">The total number of references removed from the pool.</param>
  public ReferencePoolInformation(Type type,
                                  int unusedReferenceCount,
                                  int usingReferenceCount,
                                  int acquireReferenceCount,
                                  int releaseReferenceCount,
                                  int addReferenceCount,
                                  int removeReferenceCount) {
    Type = type;
    UnusedReferenceCount = unusedReferenceCount;
    UsingReferenceCount = usingReferenceCount;
    AcquireReferenceCount = acquireReferenceCount;
    ReleaseReferenceCount = releaseReferenceCount;
    AddReferenceCount = addReferenceCount;
    RemoveReferenceCount = removeReferenceCount;
  }
}
