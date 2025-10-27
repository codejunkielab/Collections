namespace CodeJunkie.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8618

/// <summary>
/// リンクリストの範囲を表す構造体
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct LinkedListRange<T> : IEnumerable<T>, IEnumerable {
  /// <summary>
  /// ループアクセス。
  /// </summary>
  [StructLayout(LayoutKind.Auto)]
  public struct Enumerator : IEnumerator<T>, IEnumerator {
    private readonly LinkedListRange<T> _linkedListRange;
    private LinkedListNode<T> _current;
    private T _currentValue;

    internal Enumerator(LinkedListRange<T> range) {
      if (!range.IsValid) {
        throw new InvalidOperationException("Invalid range argument");
      }

      _linkedListRange = range;
      _current = _linkedListRange._first;
      _currentValue = default(T);
    }

    /// <summary>
    /// 現在のノードを返す
    /// </summary>
    public T Current => _currentValue;

    /// <summary>
    /// 現在のノードを返す
    /// </summary>
    object IEnumerator.Current => _currentValue;

    /// <summary>
    /// 列挙中のループをクリアする
    /// </summary>
    public void Dispose() {}

    /// <summary>
    /// 次のノードに移動する
    /// </summary>
    /// <returns>
    /// 移動の成功判定。以下の値が返ります
    /// false 移動に失敗したときに返ります
    /// true 移動に成功したときに返ります
    /// </returns>
    public bool MoveNext() {
      if (_current == null || _current == _linkedListRange._last) {
        return false;
      }

      _currentValue = _current.Value;
      _current = _current.Next;
      return true;
    }

    /// <summary>
    /// 列挙中のループをリセットする
    /// </summary>
    void IEnumerator.Reset() {
      _current = _linkedListRange._first;
      _currentValue = default(T);
    }
  }

  private readonly LinkedListNode<T> _first;
  private readonly LinkedListNode<T> _last;

  /// <summary>
  /// ノードの始点と終点を指定してインスタンスの初期化を行います
  /// </summary>
  /// <param name="first">リンクされたテーブルの開始ノード</param>
  /// <param name="last">リンクされたテーブルの終端ノード</param>
  public LinkedListRange(LinkedListNode<T> first, LinkedListNode<T> last) {
    if (first == null || last == null || first == last) {
      throw new ArgumentException("Invalid range argument");
    }

    _first = first;
    _last = last;
  }

  /// <summary>
  /// このリンクリストの有無
  /// </summary>
  public bool IsValid => _first != null && _last != null && _first != _last;

  /// <summary>
  /// リンクリストの開始ノードを返す
  /// </summary>
  public LinkedListNode<T> First => _first;

  /// <summary>
  /// リンクリストの終端ノードを返す
  /// </summary>
  public LinkedListNode<T> Last => _last;

  /// <summary>
  /// リンクリストのノード数を返す
  /// </summary>
  public int Count {
    get {
      if (!IsValid) {
        return 0;
      }

      int count = 0;
      for (LinkedListNode<T> current = _first; current != null && current != _last; current = current.Next) {
        count++;
      }

      return count;
    }
  }

  /// <summary>
  /// リンクリスト内に指定の値が含まれているか判定する
  /// </summary>
  /// <param name="value">指定の値</param>
  /// <returns>
  /// 指定の値が含まれてるかの判定。以下の値が返ります
  /// false 指定の値の存在が確認できなかったときに返ります
  /// true リンクリストで指定の値が保持されていたときに返ります
  /// </returns>
  public bool Contains(T value) {
    for (LinkedListNode<T> current = _first; current != null && current != _last; current = current.Next) {
      if (current?.Value.Equals(value) == true) {
        return true;
      }
    }

    return false;
  }

  /// <summary>
  /// ループアクセスを返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  public Enumerator GetEnumerator() {
    return new Enumerator(this);
  }

  /// <summary>
  /// ループアクセスを返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  IEnumerator<T> IEnumerable<T>.GetEnumerator() {
    return GetEnumerator();
  }

  /// <summary>
  /// ループアクセスを返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}
#pragma warning restore CS8601
#pragma warning restore CS8602
#pragma warning restore CS8603
#pragma warning restore CS8618
