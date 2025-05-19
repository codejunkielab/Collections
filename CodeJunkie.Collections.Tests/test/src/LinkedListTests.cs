namespace CodeJunkie.Collections.Tests;

using System;
using Xunit;
using Shouldly;
using CodeJunkie.Collections;

public class LinkedListTests {
  [Fact]
  public void AddFirst_ShouldAddElementToTheBeginning() {
    var linkedList = new LinkedList<int>();
    linkedList.AddFirst(10);
    linkedList.First.ShouldNotBeNull();
    linkedList.First.Value.ShouldBe(10);
  }

  [Fact]
  public void AddLast_ShouldAddElementToTheEnd() {
    var linkedList = new LinkedList<int>();
    linkedList.AddLast(20);
    linkedList.Last.ShouldNotBeNull();
    linkedList.Last.Value.ShouldBe(20);
  }

  [Fact]
  public void RemoveFirst_ShouldRemoveTheFirstElement() {
    var linkedList = new LinkedList<int>();
    linkedList.AddFirst(30);
    linkedList.RemoveFirst();
    linkedList.First.ShouldBeNull();
  }

  [Fact]
  public void RemoveLast_ShouldRemoveTheLastElement() {
    var linkedList = new LinkedList<int>();
    linkedList.AddLast(40);
    linkedList.RemoveLast();
    linkedList.Last.ShouldBeNull();
  }

  [Fact]
  public void Contains_ShouldReturnTrueIfElementExists() {
    var linkedList = new LinkedList<int>();
    linkedList.AddLast(50);
    var result = linkedList.Contains(50);
    result.ShouldBeTrue();
  }

  [Fact]
  public void Clear_ShouldRemoveAllElements() {
    var linkedList = new LinkedList<int>();
    linkedList.AddLast(60);
    linkedList.AddLast(70);
    linkedList.Clear();
    linkedList.Count.ShouldBe(0);
  }
}
