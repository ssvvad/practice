using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Обобщённый стек на динамическом массиве. Использует принцип LIFO: последним пришёл — первым вышел.
/// </summary>
/// <typeparam name="T">Тип элементов стека.</typeparam>
public class SmartStack<T> : IEnumerable<T>
{
    // Внутренний массив для хранения элементов.
    private T[] items;
    // Количество занятых ячеек; также индекс следующей свободной позиции.
    private int count;

    /// <summary>
    /// Количество фактически добавленных элементов.
    /// </summary>
    public int Count => count;
    /// <summary>
    /// Текущая ёмкость внутреннего массива.
    /// </summary>
    public int Capacity => items.Length;

    /// <summary>
    /// Создаёт пустой стек с начальной ёмкостью 4.
    /// </summary>
    public SmartStack() : this(4)
    {
    }

    /// <summary>
    /// Создаёт пустой стек с заданной ёмкостью.
    /// </summary>
    /// <param name="capacity">Начальный размер массива.</param>
    /// <exception cref="ArgumentOutOfRangeException">Передана отрицательная ёмкость.</exception>
    public SmartStack(int capacity)
    {
        // Отрицательный размер массива недопустим.
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        // Выделяем память и устанавливаем количество элементов равным нулю.
        items = new T[capacity];
        count = 0;
    }

    /// <summary>
    /// Создаёт стек из последовательности элементов и уменьшает ёмкость до их количества.
    /// </summary>
    /// <param name="collection">Исходная последовательность элементов.</param>
    public SmartStack(IEnumerable<T> collection) : this(4)
    {
        // Добавляем коллекцию в порядке перечисления.
        PushRange(collection);
        // Убираем неиспользуемую ёмкость после заполнения.
        Array.Resize(ref items, count);
    }

    /// <summary>
    /// Добавляет элемент на вершину стека, при необходимости расширяя массив.
    /// </summary>
    /// <param name="item">Добавляемый элемент.</param>
    public void Push(T item)
    {
        // Если массив заполнен, увеличиваем его ёмкость.
        if (count == items.Length)
        {
            // Для пустого массива выбираем размер 1; иначе удваиваем размер.
            int newCapacity = items.Length == 0
                ? 1
                : items.Length * 2;

            Array.Resize(ref items, newCapacity);
        }

        // Записываем новый элемент в первую свободную ячейку.
        items[count] = item;
        count++;
    }

    /// <summary>
    /// Последовательно добавляет все элементы переданной коллекции.
    /// </summary>
    /// <param name="collection">Последовательность добавляемых элементов.</param>
    /// <exception cref="ArgumentNullException">Коллекция равна null.</exception>
    public void PushRange(IEnumerable<T> collection)
    {
        // Проверяем, что коллекция существует.
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        // Временный буфер позволяет сначала полностью перечислить источник, в том числе сам стек.
        T[] buffer = new T[4];
        int bufferCount = 0;

        // Копируем элементы в буфер, при необходимости расширяя его.
        foreach (T item in collection)
        {
            if (bufferCount == buffer.Length)
                Array.Resize(ref buffer, buffer.Length * 2);

            buffer[bufferCount] = item;
            bufferCount++;
        }

        // Переносим накопленные элементы в стек через обычный Push.
        for (int i = 0; i < bufferCount; i++)
            Push(buffer[i]);
    }

    /// <summary>
    /// Удаляет и возвращает верхний элемент стека.
    /// </summary>
    /// <returns>Последний добавленный элемент.</returns>
    /// <exception cref="InvalidOperationException">Стек пуст.</exception>
    public T Pop()
    {
        // Из пустого стека невозможно удалить элемент.
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        // Сдвигаем вершину на один элемент вниз.
        count--;
        T item = items[count];
        // Очищаем освободившуюся ячейку, чтобы не удерживать лишние ссылки.
        items[count] = default!;

        return item;
    }

    /// <summary>
    /// Возвращает верхний элемент без удаления.
    /// </summary>
    /// <returns>Элемент на вершине.</returns>
    /// <exception cref="InvalidOperationException">Стек пуст.</exception>
    public T Peek()
    {
        // Для пустого стека вершина отсутствует.
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        return items[count - 1];
    }

    /// <summary>
    /// Проверяет наличие элемента в стеке.
    /// </summary>
    /// <param name="item">Искомое значение.</param>
    /// <returns>true, если равный элемент найден; иначе false.</returns>
    public bool Contains(T item)
    {
        // Линейный поиск по занятым ячейкам массива.
        for (int i = 0; i < count; i++)
        {
            // object.Equals сравнивает значения, включая возможные null.
            if (object.Equals(items[i], item))
                return true;
        }

        // Ни один элемент не совпал с искомым.
        return false;
    }

    /// <summary>
    /// Индексатор для чтения и изменения элементов относительно вершины стека.
    /// </summary>
    /// <param name="index">Индекс от вершины, начиная с нуля.</param>
    /// <exception cref="ArgumentOutOfRangeException">Индекс находится вне границ стека.</exception>
    public T this[int index]
    {
        get
        {
            // Проверяем границы при чтении элемента.
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // Преобразуем индекс от вершины в индекс массива.
            return items[count - 1 - index];
        }

        set
        {
            // Проверяем границы при записи элемента.
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // Заменяем элемент без изменения размера стека.
            items[count - 1 - index] = value;
        }
    }

    /// <summary>
    /// Создаёт перечислитель для обхода от вершины к основанию.
    /// </summary>
    /// <returns>Обобщённый перечислитель элементов.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        // yield return выдаёт элементы по одному в порядке LIFO.
        for (int i = count - 1; i >= 0; i--)
            yield return items[i];
    }

    /// <summary>
    /// Реализует необобщённый интерфейс IEnumerable, используя тот же порядок обхода.
    /// </summary>
    /// <returns>Необобщённый перечислитель.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Демонстрирует операции обобщённого стека.
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа: проверяет добавление, удаление, поиск, индексатор, перечисление и обработку исключений.
    /// </summary>
    static void Main()
    {
        // Создаём стек целых чисел с ёмкостью по умолчанию.
        SmartStack<int> stack = new SmartStack<int>();

        Console.WriteLine($"Начальная ёмкость: {stack.Capacity}");

        // Пятый элемент приводит к увеличению начальной ёмкости.
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        stack.Push(50);

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        // Добавляем несколько элементов за один вызов.
        stack.PushRange(new int[] { 60, 70, 80 });

        Console.WriteLine("Содержимое стека:");

        // foreach использует перечислитель стека.
        foreach (int item in stack)
            Console.Write($"{item} ");

        Console.WriteLine();

        // Сравниваем просмотр вершины и удаление элемента.
        Console.WriteLine($"Вершина: {stack.Peek()}");
        Console.WriteLine($"Удалённый элемент: {stack.Pop()}");
        Console.WriteLine($"Новая вершина: {stack.Peek()}");

        // Проверяем существующий и отсутствующий элементы.
        Console.WriteLine($"Содержит 30: {stack.Contains(30)}");
        Console.WriteLine($"Содержит 100: {stack.Contains(100)}");

        // Индекс 1 соответствует второму элементу от вершины.
        Console.WriteLine($"Элемент по индексу 1: {stack[1]}");

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        // Демонстрируем конструктор с заданной ёмкостью.
        SmartStack<int> secondStack = new SmartStack<int>(10);

        Console.WriteLine(
            $"Ёмкость второго стека: {secondStack.Capacity}");

        // Демонстрируем конструктор из коллекции.
        SmartStack<int> thirdStack =
            new SmartStack<int>(new int[] { 1, 2, 3 });

        Console.WriteLine("Третий стек:");

        foreach (int item in thirdStack)
            Console.Write($"{item} ");

        Console.WriteLine();

        // Проверяем исключение при попытке удалить элемент из пустого стека.
        try
        {
            SmartStack<int> emptyStack = new SmartStack<int>();
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        // Проверяем исключение при обращении по недопустимому индексу.
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