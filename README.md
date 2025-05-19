# Collections

Lightweight collections, utilities, and generic interface types help in creating highly maintainable code.

## Installation

Install the latest version of the [CodeJunkie.Collections] package from nuget:

```sh
dotnet add package CodeJunkie.Collections
```

## Namespace

`CodeJunkie.Collections`

## Features

- **Balckboard**: The `Blackboard` class is a shared data management system that maps types to their corresponding objects, allowing for efficient storage and retrieval.
- **Boxless Queue**: The `BoxlessQueue` class is a specialized queue designed to store and process multiple struct types without boxing, minimizing heap allocations and improving performance.
- **Set and IReadOnlySet**: The `Set` class is a custom implementation of a set that extends `HashSet<T>` and adheres to the `IReadOnlySet<T>` interface for enhanced functionality and immutability.
- **Task Pool**: The `TaskPool` class is a task management system that efficiently handles task execution, prioritization, and resource allocation using a pool of task agents.
- **Reference Pool**: The `ReferencePool` class provides a mechanism for efficient object pooling, reducing memory allocation and improving performance by managing reusable object instances.

## Key Components

### `Blackboard`

Below is an example of how to use the `Blackboard` class:

```csharp
using System;
using CodeJunkie.Collections;

public class Program {
  public static void Main() {
    // Create a new instance of the Blackboard
    var blackboard = new Blackboard();

    // Add data to the blackboard
    blackboard.Set<string>("Hello, Blackboard!");
    blackboard.Set<int>(42);

    // Retrieve data from the blackboard
    string message = blackboard.Get<string>();
    int number = blackboard.Get<int>();

    Console.WriteLine($"Message: {message}"); // Output: Message: Hello, Blackboard!
    Console.WriteLine($"Number: {number}");   // Output: Number: 42

    // Check if a type exists in the blackboard
    bool hasString = blackboard.Has<string>();
    bool hasDouble = blackboard.Has<double>();

    Console.WriteLine($"Has string: {hasString}"); // Output: Has string: True
    Console.WriteLine($"Has double: {hasDouble}"); // Output: Has double: False

    // Overwrite existing data
    blackboard.Overwrite<string>("Updated Message");
    Console.WriteLine($"Updated Message: {blackboard.Get<string>()}"); // Output: Updated Message: Updated Message

    // Attempt to retrieve a non-existent type (throws KeyNotFoundException)
    try {
      var nonExistent = blackboard.Get<double>();
    } catch (KeyNotFoundException ex) {
      Console.WriteLine($"Error: {ex.Message}");
    }
  }
}
```

#### **Explanation of Key Methods**

1. **Set<TData>(TData data)**:
   - Adds a new object of type `TData` to the blackboard.
   - Throws an exception if data of the same type already exists.

2. **Get<TData>()**:
   - Retrieves an object of type `TData` from the blackboard.
   - Throws a `KeyNotFoundException` if the type is not found.

3. **Has<TData>()**:
   - Checks if an object of type `TData` exists in the blackboard.

4. **Overwrite<TData>(TData data)**:
   - Overwrites existing data of type `TData` in the blackboard.

This example demonstrates how the `Blackboard` class can be used to manage shared data in a type-safe and flexible manner.

---

### `BoxlessQueue`
The `BoxlessQueue` class in the `CodeJunkie.Collections` namespace is a specialized queue designed to store multiple struct types without incurring the overhead of boxing and unboxing operations. This reduces heap allocations and improves performance, making it ideal for scenarios where high-performance struct handling is required. It dynamically creates internal queues for each struct type and processes dequeued values using a handler.

#### How to Use `BoxlessQueue`

1. **Creating a BoxlessQueue:**
   To use `BoxlessQueue`, you need to provide an implementation of the `IBoxlessValueHandler` interface, which defines how dequeued values are processed.
   ```csharp
   var handler = new MyBoxlessValueHandler();
   var queue = new BoxlessQueue(handler);
   ```

2. **Enqueuing Values:**
   Use the `Enqueue` method to add struct values to the queue. Each struct type gets its own internal queue.
   ```csharp
   queue.Enqueue(42);          // Enqueue an integer
   queue.Enqueue(3.14f);       // Enqueue a float
   queue.Enqueue(new Vector2(1, 2)); // Enqueue a custom struct
   ```

3. **Dequeuing Values:**
   Use the `Dequeue` method to process the next value in the queue. The value is passed to the handler without boxing.
   ```csharp
   queue.Dequeue(); // Processes the first value in the queue
   ```

4. **Checking for Values:**
   Use the `HasValues` property to check if the queue contains any values.
   ```csharp
   if (queue.HasValues)
   {
       Console.WriteLine("The queue has values to process.");
   }
   ```

5. **Clearing the Queue:**
   Use the `Clear` method to remove all values from the queue.
   ```csharp
   queue.Clear();
   ```

---

### Example

```csharp
using CodeJunkie.Collections;
using System;

public struct Vector2
{
    public float X { get; }
    public float Y { get; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
}

public class MyBoxlessValueHandler : IBoxlessValueHandler
{
    public void HandleValue<TValue>(in TValue value) where TValue : struct
    {
        Console.WriteLine($"Processing value: {value}");
    }
}

class Program
{
    static void Main()
    {
        var handler = new MyBoxlessValueHandler();
        var queue = new BoxlessQueue(handler);

        queue.Enqueue(42);
        queue.Enqueue(3.14f);
        queue.Enqueue(new Vector2(1, 2));

        while (queue.HasValues)
        {
            queue.Dequeue();
        }
    }
}
```

**Output:**
```
Processing value: 42
Processing value: 3.14
Processing value: Vector2 { X = 1, Y = 2 }
```

---

The `BoxlessQueue` is a powerful tool for managing struct-based data in performance-critical applications, ensuring minimal memory overhead and efficient processing.


### `Set` and `IReadOnlySet`

The `Set<T>` class in the `CodeJunkie.Collections` namespace is a specialized collection that extends the functionality of the standard `HashSet<T>` while also implementing the `IReadOnlySet<T>` interface. It is designed to store unique elements and provides efficient operations for adding, removing, and checking the existence of elements.

#### Usage

1. **Creating a Set:**
   You can create a `Set<T>` instance by specifying the type of elements it will store.
   ```csharp
   var mySet = new Set<int>();
   ```

2. **Adding Elements:**
   Use the `Add` method to insert elements into the set. Duplicate elements will not be added.
   ```csharp
   mySet.Add(1);
   mySet.Add(2);
   mySet.Add(1); // Duplicate, will not be added
   ```

3. **Removing Elements:**
   Use the `Remove` method to delete specific elements from the set.
   ```csharp
   mySet.Remove(1);
   ```

4. **Checking for Elements:**
   Use the `Contains` method to check if an element exists in the set.
   ```csharp
   if (mySet.Contains(2))
   {
       Console.WriteLine("Element exists in the set.");
   }
   ```

5. **Iterating Over Elements:**
   You can iterate over the elements in the set using a `foreach` loop.
   ```csharp
   foreach (var item in mySet)
   {
       Console.WriteLine(item);
   }
   ```

6. **Read-Only Access:**
   Since `Set<T>` implements `IReadOnlySet<T>`, you can use it in contexts where read-only access to the set is required.

### Example:
```csharp
using CodeJunkie.Collections;

var mySet = new Set<string>();
mySet.Add("Apple");
mySet.Add("Banana");
mySet.Add("Apple"); // Duplicate, will not be added

Console.WriteLine("Set contains:");
foreach (var item in mySet)
{
    Console.WriteLine(item);
}

// Output:
// Set contains:
// Apple
// Banana
```

The `Set<T>` class is ideal for scenarios where you need a collection of unique elements with efficient operations for membership testing and modification.

### `ReferencePool`

A static class that serves as the entry point for managing reference pools.

- **Acquire**: Retrieve an object from the pool.
- **Release**: Return an object to the pool after use.
- **Add**: Preload objects into the pool.
- **Remove**: Remove objects from the pool.
- **ClearAll**: Clear all reference pools.

#### `ReferenceCollection`
An internal class that manages a specific type of reference.

- Tracks usage statistics such as:
  - Unused references
  - References in use
  - Total acquired, released, added, and removed references

#### `ReferencePoolInformation`
A struct that provides detailed information about a specific reference pool.

- Includes properties for:
  - Reference type
  - Counts for unused, in-use, acquired, released, added, and removed references

#### Usage

##### Acquiring and Releasing References

```csharp
// Acquire a reference of type MyReference
var reference = ReferencePool.Acquire<MyReference>();

// Use the reference
reference.DoSomething();

// Release the reference back to the pool
ReferencePool.Release(reference);
```
