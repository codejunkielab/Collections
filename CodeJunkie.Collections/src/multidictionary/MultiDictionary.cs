namespace CodeJunkie.Collections;

using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

/// <summary>
/// 多重マップクラス
/// </summary>
/// <typeparam name="TKey">多重マップのキータイプ</typeparam>
/// <typeparam name="TValue">多重マップの値</typeparam>
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8618
public sealed class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, LinkedListRange<TValue>>>, IEnumerable {
  /// <summary>
  /// コレクションの列挙カウントへのループアクセス
  /// </summary>
  [StructLayout(LayoutKind.Auto)]
  public struct Enumerator : IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>>, IEnumerator {
    private Dictionary<TKey, LinkedListRange<TValue>>.Enumerator _enumerator;

    internal Enumerator(Dictionary<TKey, LinkedListRange<TValue>> dictionary) {
      if (dictionary == null) {
        throw new Exception("Invalid dictionary argument");
      }

      _enumerator = dictionary.GetEnumerator();
    }

    /// <summary>
    /// 現在のノードを返す
    /// </summary>
    public KeyValuePair<TKey, LinkedListRange<TValue>> Current => _enumerator.Current;

    /// <summary>
    /// 現在のノードを返す
    /// </summary>
    object IEnumerator.Current => _enumerator.Current;

    /// <summary>
    /// 列挙中のループをクリアする
    /// </summary>
    public void Dispose() {
      _enumerator.Dispose();
    }

    /// <summary>
    /// 次のノードに移動する
    /// </summary>
    /// <returns>
    /// 移動の成功判定。以下の値が返ります
    /// false 移動に失敗したときに返ります
    /// true 移動に成功したときに返ります
    /// </returns>
    public bool MoveNext() {
      return _enumerator.MoveNext();
    }

    /// <summary>
    /// 列挙中のループをリセットする
    /// </summary>
    void IEnumerator.Reset() {
      ((IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>>)_enumerator).Reset();
    }
  }

  private readonly LinkedList<TValue> _linkedList;
  private readonly Dictionary<TKey, LinkedListRange<TValue>> _dictionary;

  /// <summary>
  /// デフォルトコンストラクタ
  /// </summary>
  public MultiDictionary() {
    _linkedList = new LinkedList<TValue>();
    _dictionary = new Dictionary<TKey, LinkedListRange<TValue>>();
  }

  /// <summary>
  /// 多重マップに実際に含まれているキーの数を返す
  /// 多値辞書に実際に含まれる主キーの数を取得します。
  /// </summary>
  public int Count => _dictionary.Count;

  /// <summary>
  /// 多重マップの指定されたキーの範囲を返す
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <returns>プライマリキーの範囲</returns>
  public LinkedListRange<TValue> this[TKey key] {
    get {
      var range = default(LinkedListRange<TValue>);
      _dictionary.TryGetValue(key, out range);
      return range;
    }
  }

  /// <summary>
  /// 多重マップをクリーンアップする
  /// </summary>
  public void Clear() {
    _dictionary.Clear();
    _linkedList.Clear();
  }

  /// <summary>
  /// 多重マップに指定キーが含まれているか確認する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <returns>
  /// キーが含まれているかの判定。以下の値が返ります
  /// false 指定のキーが含まれていないときに返ります
  /// true 指定のキーが含まれているときに返ります
  /// </returns>
  public bool Contains(TKey key) {
    return _dictionary.ContainsKey(key);
  }

  /// <summary>
  /// 多重マップに指定した値が含まれているか確認する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <param name="value">指定値</param>
  /// <returns>
  /// 値が含まれているかの判定。以下の値が返ります
  /// false 指定の値が含まれていないときに返ります
  /// true 指定の値が含まれているときに返ります
  /// </returns>
  public bool Contains(TKey key, TValue value) {
    var range = default(LinkedListRange<TValue>);
    if (_dictionary.TryGetValue(key, out range)) {
      return range.Contains(value);
    }

    return false;
  }

  /// <summary>
  /// 指定キーから値を取得する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <param name="range">値の格納先</param>
  /// <returns>
  /// 取得の成功判定。以下の値が返ります
  /// false 取得に失敗したときに返ります
  /// true 取得に成功したときに返ります
  /// </returns>
  public bool TryGetValue(TKey key, out LinkedListRange<TValue> range) {
    return _dictionary.TryGetValue(key, out range);
  }

  /// <summary>
  /// 指定キーの値を追加する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <param name="value">追加する値</param>
  public void Add(TKey key, TValue value) {
    var range = default(LinkedListRange<TValue>);
    if (_dictionary.TryGetValue(key, out range)) {
      _linkedList.AddBefore(range.Last, value);
    } else {
      LinkedListNode<TValue> first = _linkedList.AddLast(value);
      LinkedListNode<TValue> terminal = _linkedList.AddLast(default(TValue)!);
      _dictionary.Add(key, new LinkedListRange<TValue>(first, terminal));
    }
  }

  /// <summary>
  /// 指定キーの指定した値を削除する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <param name="value">削除する値</param>
  /// <returns>
  /// 削除の成功判定。以下の値が返ります
  /// false 何かしらの理由で削除に失敗したときに返ります
  /// true 削除に成功したときに返ります
  /// </returns>
  public bool Remove(TKey key, TValue value) {
    var range = default(LinkedListRange<TValue>);
    if (_dictionary.TryGetValue(key, out range)) {
      for (LinkedListNode<TValue> current = range.First; current != null && current != range.Last; current = current.Next) {
        if (current.Value.Equals(value)) {
          if (current == range.First) {
            LinkedListNode<TValue> next = current.Next;
            if (next == range.Last) {
              _linkedList.Remove(next);
              _dictionary.Remove(key);
            } else {
              _dictionary[key] = new LinkedListRange<TValue>(next, range.Last);
            }
          }

          _linkedList.Remove(current);
          return true;
        }
      }
    }

    return false;
  }

  /// <summary>
  /// 指定したキーの全ての値を削除する
  /// </summary>
  /// <param name="key">指定キー</param>
  /// <returns>
  /// 削除の成功判定。以下の値が返ります
  /// false 何かしらの理由で削除に失敗したときに返ります
  /// true 削除に成功したときに返ります
  /// </returns>
  public bool RemoveAll(TKey key) {
    var range = default(LinkedListRange<TValue>);
    if (_dictionary.TryGetValue(key, out range)) {
      _dictionary.Remove(key);

      LinkedListNode<TValue> current = range.First;
      while (current != null) {
        LinkedListNode<TValue> next = current != range.Last ? current.Next : null;
        _linkedList.Remove(current);
        current = next;
      }

      return true;
    }

    return false;
  }

  /// <summary>
  /// ループアクセスコレクションに含まれる列挙体を返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  public Enumerator GetEnumerator() {
    return new Enumerator(_dictionary);
  }

  /// <summary>
  /// ループアクセスコレクションに含まれる列挙体を返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  IEnumerator<KeyValuePair<TKey, LinkedListRange<TValue>>> IEnumerable<KeyValuePair<TKey, LinkedListRange<TValue>>>.GetEnumerator() {
    return GetEnumerator();
  }

  /// <summary>
  /// ループアクセスコレクションに含まれる列挙体を返す
  /// </summary>
  /// <returns>ループアクセス</returns>
  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}
#pragma warning restore CS8600
#pragma warning restore CS8601
#pragma warning restore CS8602
#pragma warning restore CS8603
#pragma warning restore CS8618
