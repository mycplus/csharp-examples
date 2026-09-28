using System;
using System.Collections.Generic;

namespace MyCPlus.Stack;

public static class Program
{
    public static bool Balanced(string text)
    {
        var open = new Stack<char>();
        foreach (char c in text)
        {
            switch (c)
            {
                case '(' or '[' or '{':
                    open.Push(c);
                    break;
                case ')' or ']' or '}':
                    char want = c == ')' ? '(' : c == ']' ? '[' : '{';
                    if (!open.TryPop(out char top) || top != want)
                        return false;
                    break;
            }
        }
        return open.Count == 0;
    }

    public static void Main()
    {
        var s = new ArrayStack<int>();
        Console.WriteLine("push 10 20 30");
        s.Push(10);
        s.Push(20);
        s.Push(30);
        Console.WriteLine($"size={s.Count} top={s.Peek()}");
        while (s.TryPop(out int v))
            Console.WriteLine($"pop {v}");
        Console.WriteLine($"empty={(s.IsEmpty ? "true" : "false")}");
        Console.WriteLine("pop on empty: " +
            (s.TryPop(out _) ? "returned a value" : "underflow reported"));

        foreach (string t in new[] { "{[()()]}", "([)]", "((", "())", "" })
            Console.WriteLine($"balanced(\"{t}\") = {(Balanced(t) ? "true" : "false")}");
    }
}
