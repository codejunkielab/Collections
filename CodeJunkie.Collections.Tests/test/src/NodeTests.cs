namespace CodeJunkie.Collections.Tests;

using System;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Shouldly;
using CodeJunkie.Collections;

public class NodeTests {
#region Test Data Classes

  public class TestData {
    public string Value { get; set; }
    public int Number { get; set; }
  }

  public class GameData {
    public int Score { get; set; }
    public int Level { get; set; }
  }

  public class PlayerData {
    public string Name { get; set; }
    public int Health { get; set; }
  }

#endregion

#region Constructor Tests

  [Fact]
  public void Constructor_WithDefaultParameters_ShouldCreateNodeWithDefaultNameAndOwnBlackboard() {
    // Act
    var node = new Node();

    // Assert
    node.Name.ShouldBe("Node");
    node.Parent.ShouldBeNull();
    node.Children.ShouldBeEmpty();
    node.IsRoot.ShouldBeTrue();
    node.IsLeaf.ShouldBeTrue();
    node.ChildCount.ShouldBe(0);
    node.Blackboard.ShouldNotBeNull();
  }

  [Fact]
  public void Constructor_WithCustomName_ShouldCreateNodeWithSpecifiedNameAndOwnBlackboard() {
    // Arrange
    const string customName = "CustomNode";

    // Act
    var node = new Node(customName);

    // Assert
    node.Name.ShouldBe(customName);
    node.Blackboard.ShouldNotBeNull();
  }

  [Fact]
  public void Constructor_MultipleNodes_ShouldHaveIndependentBlackboards() {
    // Arrange & Act
    var node1 = new Node("Node1");
    var node2 = new Node("Node2");
    var testData = new TestData { Value = "test" };

    node1.Set(testData);

    // Assert
    node1.Blackboard.ShouldNotBe(node2.Blackboard);
    node1.Has<TestData>().ShouldBeTrue();
    node2.Has<TestData>().ShouldBeFalse();
  }

#endregion

#region Property Tests

  [Fact]
  public void Name_WhenSet_ShouldUpdateCorrectly() {
    // Arrange
    var node = new Node();
    const string newName = "NewName";

    // Act
    node.Name = newName;

    // Assert
    node.Name.ShouldBe(newName);
  }

  [Fact]
  public void Name_WhenSetToNull_ShouldThrowArgumentNullException() {
    // Arrange
    var node = new Node();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => node.Name = null);
  }

  [Fact]
  public void IsRoot_WithParent_ShouldReturnFalse() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");

    // Act
    parent.AddChild(child);

    // Assert
    child.IsRoot.ShouldBeFalse();
    parent.IsRoot.ShouldBeTrue();
  }

  [Fact]
  public void IsLeaf_WithChildren_ShouldReturnFalse() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");

    // Act
    parent.AddChild(child);

    // Assert
    parent.IsLeaf.ShouldBeFalse();
    child.IsLeaf.ShouldBeTrue();
  }

#endregion

#region Child Management Tests

  [Fact]
  public void AddChild_ValidChild_ShouldAddChildAndSetParent() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");

    // Act
    parent.AddChild(child);

    // Assert
    parent.Children.ShouldContain(child);
    parent.ChildCount.ShouldBe(1);
    child.Parent.ShouldBe(parent);
  }

  [Fact]
  public void AddChild_NullChild_ShouldThrowArgumentNullException() {
    // Arrange
    var parent = new Node("Parent");

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => parent.AddChild(null));
  }

  [Fact]
  public void AddChild_Self_ShouldThrowInvalidOperationException() {
    // Arrange
    var node = new Node("Node");

    // Act & Assert
    Should.Throw<InvalidOperationException>(() => node.AddChild(node))
      .Message.ShouldContain("Cannot add self as child");
  }

  [Fact]
  public void AddChild_Ancestor_ShouldThrowInvalidOperationException() {
    // Arrange
    var grandParent = new Node("GrandParent");
    var parent = new Node("Parent");
    var child = new Node("Child");

    grandParent.AddChild(parent);
    parent.AddChild(child);

    // Debug: Let's verify the relationships are set up correctly
    parent.Parent.ShouldBe(grandParent);
    child.Parent.ShouldBe(parent);

    // Act & Assert - child trying to add its grandparent should fail
    Should.Throw<InvalidOperationException>(() => child.AddChild(grandParent))
      .Message.ShouldContain("would create cycle");
  }

  [Fact]
  public void AddChild_ChildWithExistingParent_ShouldMoveChild() {
    // Arrange
    var oldParent = new Node("OldParent");
    var newParent = new Node("NewParent");
    var child = new Node("Child");

    oldParent.AddChild(child);

    // Act
    newParent.AddChild(child);

    // Assert
    oldParent.Children.ShouldNotContain(child);
    newParent.Children.ShouldContain(child);
    child.Parent.ShouldBe(newParent);
  }

  [Fact]
  public void RemoveChild_ValidChild_ShouldRemoveChildAndClearParent() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    parent.AddChild(child);

    // Act
    var result = parent.RemoveChild(child);

    // Assert
    result.ShouldBeTrue();
    parent.Children.ShouldNotContain(child);
    child.Parent.ShouldBeNull();
  }

  [Fact]
  public void RemoveChild_InvalidChild_ShouldReturnFalse() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");

    // Act
    var result = parent.RemoveChild(child);

    // Assert
    result.ShouldBeFalse();
  }

  [Fact]
  public void GetChild_ValidIndex_ShouldReturnCorrectChild() {
    // Arrange
    var parent = new Node("Parent");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");

    parent.AddChild(child1);
    parent.AddChild(child2);

    // Act & Assert
    parent.GetChild(0).ShouldBe(child1);
    parent.GetChild(1).ShouldBe(child2);
  }

  [Fact]
  public void GetChild_InvalidIndex_ShouldThrowArgumentOutOfRangeException() {
    // Arrange
    var parent = new Node("Parent");

    // Act & Assert
    Should.Throw<ArgumentOutOfRangeException>(() => parent.GetChild(0));
    Should.Throw<ArgumentOutOfRangeException>(() => parent.GetChild(-1));
  }

  [Fact]
  public void GetChild_ByName_ShouldReturnCorrectChild() {
    // Arrange
    var parent = new Node("Parent");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");

    parent.AddChild(child1);
    parent.AddChild(child2);

    // Act & Assert
    parent.GetChild("Child1").ShouldBe(child1);
    parent.GetChild("Child2").ShouldBe(child2);
    parent.GetChild("NonExistent").ShouldBeNull();
  }

  [Fact]
  public void RemoveAllChildren_ShouldRemoveAllChildrenAndClearTheirParents() {
    // Arrange
    var parent = new Node("Parent");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");

    parent.AddChild(child1);
    parent.AddChild(child2);

    // Act
    parent.RemoveAllChildren();

    // Assert
    parent.Children.ShouldBeEmpty();
    child1.Parent.ShouldBeNull();
    child2.Parent.ShouldBeNull();
  }

#endregion

#region Hierarchy Navigation Tests

  [Fact]
  public void GetRoot_FromDeepNode_ShouldReturnRootNode() {
    // Arrange
    var root = new Node("Root");
    var child = new Node("Child");
    var grandChild = new Node("GrandChild");

    root.AddChild(child);
    child.AddChild(grandChild);

    // Act & Assert
    grandChild.GetRoot().ShouldBe(root);
    child.GetRoot().ShouldBe(root);
    root.GetRoot().ShouldBe(root);
  }

  [Fact]
  public void IsAncestorOf_ValidRelationship_ShouldReturnTrue() {
    // Arrange
    var grandParent = new Node("GrandParent");
    var parent = new Node("Parent");
    var child = new Node("Child");

    grandParent.AddChild(parent);
    parent.AddChild(child);

    // Act & Assert
    grandParent.IsAncestorOf(child).ShouldBeTrue();
    parent.IsAncestorOf(child).ShouldBeTrue();
    child.IsAncestorOf(grandParent).ShouldBeFalse();

    // Debug: Let's also test the reverse relationship that should cause the exception
    child.IsAncestorOf(grandParent).ShouldBeFalse();
    grandParent.IsAncestorOf(child).ShouldBeTrue(); // This should be true
  }

  [Fact]
  public void IsDescendantOf_ValidRelationship_ShouldReturnTrue() {
    // Arrange
    var grandParent = new Node("GrandParent");
    var parent = new Node("Parent");
    var child = new Node("Child");

    grandParent.AddChild(parent);
    parent.AddChild(child);

    // Act & Assert
    child.IsDescendantOf(grandParent).ShouldBeTrue();
    child.IsDescendantOf(parent).ShouldBeTrue();
    grandParent.IsDescendantOf(child).ShouldBeFalse();
  }

  [Fact]
  public void GetNodeByPath_ValidPath_ShouldReturnCorrectNode() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild = new Node("GrandChild");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild);

    // Act & Assert
    root.GetNodeByPath("Child1").ShouldBe(child1);
    root.GetNodeByPath("Child1/GrandChild").ShouldBe(grandChild);
    root.GetNodeByPath("").ShouldBe(root);
    root.GetNodeByPath("NonExistent").ShouldBeNull();
  }

  [Fact]
  public void GetPath_ShouldReturnCorrectPath() {
    // Arrange
    var root = new Node("Root");
    var child = new Node("Child");
    var grandChild = new Node("GrandChild");

    root.AddChild(child);
    child.AddChild(grandChild);

    // Act & Assert
    root.GetPath().ShouldBe("Root");
    child.GetPath().ShouldBe("Root/Child");
    grandChild.GetPath().ShouldBe("Root/Child/GrandChild");
  }

  [Fact]
  public void GetDepth_ShouldReturnCorrectDepth() {
    // Arrange
    var root = new Node("Root");
    var child = new Node("Child");
    var grandChild = new Node("GrandChild");

    root.AddChild(child);
    child.AddChild(grandChild);

    // Act & Assert
    root.GetDepth().ShouldBe(0);
    child.GetDepth().ShouldBe(1);
    grandChild.GetDepth().ShouldBe(2);
  }

#endregion

#region Tree Traversal Tests

  [Fact]
  public void GetAllDescendants_ShouldReturnAllDescendantsInDepthFirstOrder() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild1 = new Node("GrandChild1");
    var grandChild2 = new Node("GrandChild2");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild1);
    child1.AddChild(grandChild2);

    // Act
    var descendants = root.GetAllDescendants().ToList();

    // Assert
    descendants.Count.ShouldBe(4);
    descendants[0].ShouldBe(child1);
    descendants[1].ShouldBe(grandChild1);
    descendants[2].ShouldBe(grandChild2);
    descendants[3].ShouldBe(child2);
  }

  [Fact]
  public void GetAllDescendantsBreadthFirst_ShouldReturnAllDescendantsInBreadthFirstOrder() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild1 = new Node("GrandChild1");
    var grandChild2 = new Node("GrandChild2");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild1);
    child1.AddChild(grandChild2);

    // Act
    var descendants = root.GetAllDescendantsBreadthFirst().ToList();

    // Assert
    descendants.Count.ShouldBe(4);
    descendants[0].ShouldBe(child1);
    descendants[1].ShouldBe(child2);
    descendants[2].ShouldBe(grandChild1);
    descendants[3].ShouldBe(grandChild2);
  }

  [Fact]
  public void FindChild_WithValidPredicate_ShouldReturnFirstMatch() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild = new Node("GrandChild");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild);

    // Act
    var found = root.FindChild(node => node.Name.StartsWith("Grand"));

    // Assert
    found.ShouldBe(grandChild);
  }

  [Fact]
  public void FindChild_WithNullPredicate_ShouldThrowArgumentNullException() {
    // Arrange
    var root = new Node("Root");

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => root.FindChild(null));
  }

  [Fact]
  public void FindChildren_WithValidPredicate_ShouldReturnAllMatches() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild1 = new Node("Child3");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild1);

    // Act
    var found = root.FindChildren(node => node.Name.StartsWith("Child")).ToList();

    // Assert
    found.Count.ShouldBe(3);
    found.ShouldContain(child1);
    found.ShouldContain(child2);
    found.ShouldContain(grandChild1);
  }

#endregion

#region Blackboard Tests

  [Fact]
  public void Set_ValidData_ShouldStoreDataInOwnBlackboard() {
    // Arrange
    var node = new Node("TestNode");
    var testData = new TestData { Value = "test", Number = 42 };

    // Act
    node.Set(testData);

    // Assert
    node.Blackboard.Has<TestData>().ShouldBeTrue();
    node.Get<TestData>().ShouldBe(testData);
  }

  [Fact]
  public void Get_DataNotInCurrentNodeButInParent_ShouldReturnParentData() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    var testData = new TestData { Value = "parent_data" };

    parent.AddChild(child);
    parent.Set(testData);

    // Act
    var retrievedData = child.Get<TestData>();

    // Assert
    retrievedData.ShouldBe(testData);
  }

  [Fact]
  public void Get_DataInBothCurrentNodeAndParent_ShouldReturnCurrentNodeData() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    var parentData = new TestData { Value = "parent_data" };
    var childData = new TestData { Value = "child_data" };

    parent.AddChild(child);
    parent.Set(parentData);
    child.Set(childData);

    // Act
    var retrievedData = child.Get<TestData>();

    // Assert
    retrievedData.ShouldBe(childData);
    retrievedData.Value.ShouldBe("child_data");
  }

  [Fact]
  public void Get_DataNotFoundInHierarchy_ShouldThrowKeyNotFoundException() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    parent.AddChild(child);

    // Act & Assert
    var exception = Should.Throw<KeyNotFoundException>(() => child.Get<TestData>());
    exception.Message.ShouldContain("Data of type");
    exception.Message.ShouldContain("not found in node hierarchy");
  }

  [Fact]
  public void GetObject_WithRuntimeType_ShouldWorkLikeGenericGet() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    var testData = new TestData { Value = "test" };

    parent.AddChild(child);
    parent.Set(testData);

    // Act
    var retrievedData = child.GetObject(typeof(TestData));

    // Assert
    retrievedData.ShouldBe(testData);
    ((TestData)retrievedData).Value.ShouldBe("test");
  }

  [Fact]
  public void Has_DataInHierarchy_ShouldReturnTrue() {
    // Arrange
    var grandParent = new Node("GrandParent");
    var parent = new Node("Parent");
    var child = new Node("Child");
    var testData = new TestData { Value = "test" };

    grandParent.AddChild(parent);
    parent.AddChild(child);
    grandParent.Set(testData);

    // Act & Assert
    child.Has<TestData>().ShouldBeTrue();
    parent.Has<TestData>().ShouldBeTrue();
    grandParent.Has<TestData>().ShouldBeTrue();
  }

  [Fact]
  public void Has_DataNotInHierarchy_ShouldReturnFalse() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    parent.AddChild(child);

    // Act & Assert
    child.Has<TestData>().ShouldBeFalse();
  }

  [Fact]
  public void HasObject_WithRuntimeType_ShouldWorkLikeGenericHas() {
    // Arrange
    var parent = new Node("Parent");
    var child = new Node("Child");
    var testData = new TestData { Value = "test" };

    parent.AddChild(child);
    parent.Set(testData);

    // Act & Assert
    child.HasObject(typeof(TestData)).ShouldBeTrue();
    child.HasObject(typeof(GameData)).ShouldBeFalse();
  }

  [Fact]
  public void Overwrite_ExistingData_ShouldReplaceData() {
    // Arrange
    var node = new Node("TestNode");
    var originalData = new TestData { Value = "original" };
    var newData = new TestData { Value = "new" };

    node.Set(originalData);

    // Act
    node.Overwrite(newData);

    // Assert
    node.Get<TestData>().ShouldBe(newData);
    node.Get<TestData>().Value.ShouldBe("new");
  }

#endregion

#region Integration Tests

  [Fact]
  public void ComplexHierarchyWithMultipleDataTypes_ShouldWorkCorrectly() {
    // Arrange
    var root = new Node("Root");
    var gameManager = new Node("GameManager");
    var player = new Node("Player");
    var ui = new Node("UI");

    var gameData = new GameData { Score = 1000, Level = 5 };
    var playerData = new PlayerData { Name = "TestPlayer", Health = 100 };
    var testData = new TestData { Value = "ui_data", Number = 42 };

    // Build hierarchy
    root.AddChild(gameManager);
    gameManager.AddChild(player);
    gameManager.AddChild(ui);

    // Set data at different levels
    root.Set(gameData);
    gameManager.Set(playerData);
    ui.Set(testData);

    // Act & Assert
    // Player should access GameData from root and PlayerData from gameManager
    player.Get<GameData>().Score.ShouldBe(1000);
    player.Get<PlayerData>().Name.ShouldBe("TestPlayer");

    // UI should access all data
    ui.Get<GameData>().Level.ShouldBe(5);
    ui.Get<PlayerData>().Health.ShouldBe(100);
    ui.Get<TestData>().Value.ShouldBe("ui_data");

    // GameManager shouldn't access UI's data
    gameManager.Has<TestData>().ShouldBeFalse();

    // Root shouldn't access lower level data
    root.Has<PlayerData>().ShouldBeFalse();
    root.Has<TestData>().ShouldBeFalse();
  }

  [Fact]
  public void NodeRemoval_ShouldNotAffectBlackboardAccess() {
    // Arrange
    var parent = new Node("Parent");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var testData = new TestData { Value = "shared" };

    parent.AddChild(child1);
    parent.AddChild(child2);
    parent.Set(testData);

    // Verify both children can access data
    child1.Get<TestData>().Value.ShouldBe("shared");
    child2.Get<TestData>().Value.ShouldBe("shared");

    // Act - Remove one child
    parent.RemoveChild(child1);

    // Assert - Other child should still access data
    child2.Get<TestData>().Value.ShouldBe("shared");

    // Removed child should no longer access parent data
    Should.Throw<KeyNotFoundException>(() => child1.Get<TestData>());
  }

#endregion

#region Utility Method Tests

  [Fact]
  public void PrintTree_ShouldReturnFormattedTreeStructure() {
    // Arrange
    var root = new Node("Root");
    var child1 = new Node("Child1");
    var child2 = new Node("Child2");
    var grandChild = new Node("GrandChild");

    root.AddChild(child1);
    root.AddChild(child2);
    child1.AddChild(grandChild);

    // Act
    var treeString = root.PrintTree();

    // Assert
    treeString.ShouldContain("Root");
    treeString.ShouldContain("  Child1");
    treeString.ShouldContain("  Child2");
    treeString.ShouldContain("    GrandChild");
  }

  [Fact]
  public void ToString_ShouldReturnNodeWithName() {
    // Arrange
    var node = new Node("TestNode");

    // Act
    var result = node.ToString();

    // Assert
    result.ShouldBe("Node(TestNode)");
  }

#endregion
}
