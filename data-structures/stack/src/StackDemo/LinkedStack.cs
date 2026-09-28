using System;

namespace MyCPlus.Stack;

/// <summary>A stack built on a singly linked list. Not thread-safe.</summary>
public sealed class LinkedStack<T>
{
    private sealed class Node(T value, Node? next)
    {
        public T Value { get; } = value;
        public Node? Next { get; } = next;   // the node below this one
    }

    private Node? _top;

    public int Count { get; private set; }
    public bool IsEmpty => _top is null;

    public void Push(T value)
    {
        _top = new Node(value, _top);
        Count++;
    }

    public T Pop()
    {
        T value = Peek();
        _top = _top!.Next;
        Count--;
        return value;
    }

    public T Peek()
    {
        if (_top is null)
            throw new InvalidOperationException("Pop or Peek on an empty stack.");
        return _top.Value;
    }
}
