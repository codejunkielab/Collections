namespace CodeJunkie.Collections.Tests;

using Shouldly;
using System.Collections.Generic;
using CodeJunkie.Collections;
using Xunit;

public class HashSetExtensionsTests {
  [Fact]
  public void WithIncludesItem() {
    var set = new HashSet<int> { 1, 2, 3 };
    var result = set.With(4);

    result.ShouldContain(4);
    result.Count.ShouldBe(4);
  }

  [Fact]
  public void WithoutExcludesItem() {
    var set = new HashSet<int> { 1, 2, 3 };
    var result = set.Without(2);

    result.ShouldNotContain(2);
    result.Count.ShouldBe(2);
  }
}
