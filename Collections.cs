using System;
using System.Collections;
using System.Collections.Generic;

public class SmartStack<T> : IEnumerable<T>
{
    private T[] items;
    private int count;

    public int Count => count;
    public int Capacity => items.Length;

    public SmartStack() : this(4)
    {
    }

    public SmartStack(int capacity)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        items = new T[capacity];
        count = 0;
    }

    public SmartStack(IEnumerable<T> collection) : this(4)
    {
        PushRange(collection);
        Array.Resize(ref items, count);
    }

    public void Push(T item)
    {
        if (count == items.Length)
        {
            int newCapacity = items.Length == 0
                ? 1
                : items.Length * 2;

            Array.Resize(ref items, newCapacity);
        }

        items[count] = item;
        count++;
    }

    public void PushRange(IEnumerable<T> collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        T[] buffer = new T[4];
        int bufferCount = 0;

        foreach (T item in collection)
        {
            if (bufferCount == buffer.Length)
                Array.Resize(ref buffer, buffer.Length * 2);

            buffer[bufferCount] = item;
            bufferCount++;
        }

        for (int i = 0; i < bufferCount; i++)
            Push(buffer[i]);
    }

    public T Pop()
    {
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        count--;
        T item = items[count];
        items[count] = default!;

        return item;
    }

    public T Peek()
    {
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        return items[count - 1];
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < count; i++)
        {
            if (object.Equals(items[i], item))
                return true;
        }

        return false;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return items[count - 1 - index];
        }

        set
        {
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            items[count - 1 - index] = value;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = count - 1; i >= 0; i--)
            yield return items[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

class Program
{
    static void Main()
    {
        SmartStack<int> stack = new SmartStack<int>();

        Console.WriteLine($"Начальная ёмкость: {stack.Capacity}");

        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        stack.Push(50);

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        stack.PushRange(new int[] { 60, 70, 80 });

        Console.WriteLine("Содержимое стека:");

        foreach (int item in stack)
            Console.Write($"{item} ");

        Console.WriteLine();

        Console.WriteLine($"Вершина: {stack.Peek()}");
        Console.WriteLine($"Удалённый элемент: {stack.Pop()}");
        Console.WriteLine($"Новая вершина: {stack.Peek()}");

        Console.WriteLine($"Содержит 30: {stack.Contains(30)}");
        Console.WriteLine($"Содержит 100: {stack.Contains(100)}");

        Console.WriteLine($"Элемент по индексу 1: {stack[1]}");

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        SmartStack<int> secondStack = new SmartStack<int>(10);

        Console.WriteLine(
            $"Ёмкость второго стека: {secondStack.Capacity}");

        SmartStack<int> thirdStack =
            new SmartStack<int>(new int[] { 1, 2, 3 });

        Console.WriteLine("Третий стек:");

        foreach (int item in thirdStack)
            Console.Write($"{item} ");

        Console.WriteLine();

        try
        {
            SmartStack<int> emptyStack = new SmartStack<int>();
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        try
        {
            Console.WriteLine(stack[100]);
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Ошибка: индекс вне границ стека.");
        }
    }
}