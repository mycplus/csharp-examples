using System;

namespace MyCPlus.Stack;

/// <summary>A growable array-backed stack. Not thread-safe.</summary>
public sealed class ArrayStack<T>
{
    private T[] _items = new T[8];
    private int _size;

    public int Count => _size;
    public bool IsEmpty => _size == 0;

    public void Push(T value)
    {
        if (_size == _items.Length)
        {
            // Doubling in long arithmetic cannot wrap; Array.MaxLength caps it.
            int newLength = (int)Math.Min(2L * _items.Length, Array.MaxLength);
            if (newLength == _size)
                throw new InvalidOperationException("Stack is full.");
            Array.Resize(ref _items, newLength);
        }
        _items[_size++] = value;
    }

    public T Pop()
    {
        T value = Peek();
        _items[--_size] = default!;   // drop the reference so the GC can reclaim it
        return value;
    }

    public T Peek()
    {
        if (_size == 0)
            throw new InvalidOperationException("Pop or Peek on an empty stack.");
        return _items[_size - 1];
    }

    public bool TryPop(out T value)
    {
        if (_size == 0)
        {
            value = default!;
            return false;
        }
        value = Pop();
        return true;
    }
}
