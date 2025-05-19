namespace CodeJunkie.Collections.Tests;

using CodeJunkie.Collections;
using Shouldly;
using System;
using Xunit;

public class ReferencePoolInformationTests {
  [Fact]
  public void Constructor_ShouldInitializePropertiesCorrectly() {
    // Arrange
    var type = typeof(string);
    var unusedReferenceCount = 10;
    var usingReferenceCount = 5;
    var acquireReferenceCount = 15;
    var releaseReferenceCount = 12;
    var addReferenceCount = 8;
    var removeReferenceCount = 3;

    // Act
    var info = new ReferencePoolInformation(
        type,
        unusedReferenceCount,
        usingReferenceCount,
        acquireReferenceCount,
        releaseReferenceCount,
        addReferenceCount,
        removeReferenceCount);

    // Assert
    info.Type.ShouldBe(type);
    info.UnusedReferenceCount.ShouldBe(unusedReferenceCount);
    info.UsingReferenceCount.ShouldBe(usingReferenceCount);
    info.AcquireReferenceCount.ShouldBe(acquireReferenceCount);
    info.ReleaseReferenceCount.ShouldBe(releaseReferenceCount);
    info.AddReferenceCount.ShouldBe(addReferenceCount);
    info.RemoveReferenceCount.ShouldBe(removeReferenceCount);
  }

  [Fact]
  public void Properties_ShouldReturnCorrectValues() {
    // Arrange
    var type = typeof(int);
    var unusedReferenceCount = 20;
    var usingReferenceCount = 10;
    var acquireReferenceCount = 30;
    var releaseReferenceCount = 25;
    var addReferenceCount = 15;
    var removeReferenceCount = 5;

    var info = new ReferencePoolInformation(
        type,
        unusedReferenceCount,
        usingReferenceCount,
        acquireReferenceCount,
        releaseReferenceCount,
        addReferenceCount,
        removeReferenceCount);

    // Act & Assert
    info.Type.ShouldBe(typeof(int));
    info.UnusedReferenceCount.ShouldBe(20);
    info.UsingReferenceCount.ShouldBe(10);
    info.AcquireReferenceCount.ShouldBe(30);
    info.ReleaseReferenceCount.ShouldBe(25);
    info.AddReferenceCount.ShouldBe(15);
    info.RemoveReferenceCount.ShouldBe(5);
  }
}
