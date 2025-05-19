namespace CodeJunkie.Collections.Tests;

using CodeJunkie.Collections;
using Moq;
using Shouldly;
using System;
using Xunit;

public class IReferenceTests {
  private readonly Mock<IReference> _mockContext;
  private readonly IReference _mockReference;

  public IReferenceTests() {
    _mockContext = new Mock<IReference>();
    _mockReference = _mockContext.Object;
  }

  [Fact]
  public void Clear_ShouldBeCalledOnce() {
    // Arrange
    _mockContext.Setup(m => m.Clear());

    // Act
    _mockReference.Clear();

    // Assert
    _mockContext.Verify(m => m.Clear(), Times.Once);
  }

  [Fact]
  public void Clear_ShouldNotThrowException() {
    // Arrange
    _mockContext.Setup(m => m.Clear());

    // Act & Assert
    Should.NotThrow(() => _mockReference.Clear());
  }
}
