using System;
using System.Collections;
using System.Collections.Generic;

// обобщённый стек на динамическом массиве; использует принцип LIFO: последним пришёл — первым вышел
// тип T — тип элементов стека
public class SmartStack<T> : IEnumerable<T>
{
    // внутренний массив для хранения элементов
    private T[] items;
    // количество занятых ячеек; также индекс следующей свободной позиции
    private int count;

    // количество фактически добавленных элементов
    public int Count => count;
    // текущая ёмкость внутреннего массива
    public int Capacity => items.Length;

    // создаёт пустой стек с начальной ёмкостью 4
    public SmartStack() : this(4)
    {
    }

    // создаёт пустой стек с заданной ёмкостью
    // capacity — начальный размер массива
    // исключение ArgumentOutOfRangeException — передана отрицательная ёмкость
    public SmartStack(int capacity)
    {
        // отрицательный размер массива недопустим
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        // выделяем память и устанавливаем количество элементов равным нулю
        items = new T[capacity];
        count = 0;
    }

    // создаёт стек из последовательности элементов и уменьшает ёмкость до их количества
    // collection — исходная последовательность элементов
    public SmartStack(IEnumerable<T> collection) : this(4)
    {
        // добавляем коллекцию в порядке перечисления
        PushRange(collection);
        // убираем неиспользуемую ёмкость после заполнения
        Array.Resize(ref items, count);
    }

    // добавляет элемент на вершину стека, при необходимости расширяя массив
    // item — добавляемый элемент
    public void Push(T item)
    {
        // если массив заполнен, увеличиваем его ёмкость
        if (count == items.Length)
        {
            // для пустого массива выбираем размер 1; иначе удваиваем размер
            int newCapacity = items.Length == 0
                ? 1
                : items.Length * 2;

            Array.Resize(ref items, newCapacity);
        }

        // записываем новый элемент в первую свободную ячейку
        items[count] = item;
        count++;
    }

    // последовательно добавляет все элементы переданной коллекции
    // collection — последовательность добавляемых элементов
    // исключение ArgumentNullException — коллекция равна null
    public void PushRange(IEnumerable<T> collection)
    {
        // проверяем, что коллекция существует
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        // временный буфер позволяет сначала полностью перечислить источник, в том числе сам стек
        T[] buffer = new T[4];
        int bufferCount = 0;

        // копируем элементы в буфер, при необходимости расширяя его
        foreach (T item in collection)
        {
            if (bufferCount == buffer.Length)
                Array.Resize(ref buffer, buffer.Length * 2);

            buffer[bufferCount] = item;
            bufferCount++;
        }

        // переносим накопленные элементы в стек через обычный Push
        for (int i = 0; i < bufferCount; i++)
            Push(buffer[i]);
    }

    // удаляет и возвращает верхний элемент стека
    // возвращает: последний добавленный элемент
    // исключение InvalidOperationException — стек пуст
    public T Pop()
    {
        // из пустого стека невозможно удалить элемент
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        // сдвигаем вершину на один элемент вниз
        count--;
        T item = items[count];
        // очищаем освободившуюся ячейку, чтобы не удерживать лишние ссылки
        items[count] = default!;

        return item;
    }

    // возвращает верхний элемент без удаления
    // возвращает: элемент на вершине
    // исключение InvalidOperationException — стек пуст
    public T Peek()
    {
        // для пустого стека вершина отсутствует
        if (count == 0)
            throw new InvalidOperationException("Стек пуст.");

        return items[count - 1];
    }

    // проверяет наличие элемента в стеке
    // item — искомое значение
    // возвращает: true, если равный элемент найден; иначе false
    public bool Contains(T item)
    {
        // линейный поиск по занятым ячейкам массива
        for (int i = 0; i < count; i++)
        {
            // object.Equals сравнивает значения, включая возможные null
            if (object.Equals(items[i], item))
                return true;
        }

        // ни один элемент не совпал с искомым
        return false;
    }

    // индексатор для чтения и изменения элементов относительно вершины стека
    // index — индекс от вершины, начиная с нуля
    // исключение ArgumentOutOfRangeException — индекс находится вне границ стека
    public T this[int index]
    {
        get
        {
            // проверяем границы при чтении элемента
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // преобразуем индекс от вершины в индекс массива
            return items[count - 1 - index];
        }

        set
        {
            // проверяем границы при записи элемента
            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException(nameof(index));

            // заменяем элемент без изменения размера стека
            items[count - 1 - index] = value;
        }
    }

    // создаёт перечислитель для обхода от вершины к основанию
    // возвращает: обобщённый перечислитель элементов
    public IEnumerator<T> GetEnumerator()
    {
        // yield return выдаёт элементы по одному в порядке LIFO
        for (int i = count - 1; i >= 0; i--)
            yield return items[i];
    }

    // реализует необобщённый интерфейс IEnumerable, используя тот же порядок обхода
    // возвращает: необобщённый перечислитель
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

// демонстрирует операции обобщённого стека
class Program
{
    // метод Main: проверяет добавление, удаление, поиск, индексатор, перечисление и обработку исключений
    static void Main()
    {
        // создаём стек целых чисел с ёмкостью по умолчанию
        SmartStack<int> stack = new SmartStack<int>();

        Console.WriteLine($"Начальная ёмкость: {stack.Capacity}");

        // пятый элемент приводит к увеличению начальной ёмкости
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        stack.Push(50);

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        // добавляем несколько элементов за один вызов
        stack.PushRange(new int[] { 60, 70, 80 });

        Console.WriteLine("Содержимое стека:");

        // foreach использует перечислитель стека
        foreach (int item in stack)
            Console.Write($"{item} ");

        Console.WriteLine();

        // сравниваем просмотр вершины и удаление элемента
        Console.WriteLine($"Вершина: {stack.Peek()}");
        Console.WriteLine($"Удалённый элемент: {stack.Pop()}");
        Console.WriteLine($"Новая вершина: {stack.Peek()}");

        // проверяем существующий и отсутствующий элементы
        Console.WriteLine($"Содержит 30: {stack.Contains(30)}");
        Console.WriteLine($"Содержит 100: {stack.Contains(100)}");

        // индекс 1 соответствует второму элементу от вершины
        Console.WriteLine($"Элемент по индексу 1: {stack[1]}");

        Console.WriteLine($"Количество: {stack.Count}");
        Console.WriteLine($"Ёмкость: {stack.Capacity}");

        // демонстрируем конструктор с заданной ёмкостью
        SmartStack<int> secondStack = new SmartStack<int>(10);

        Console.WriteLine(
            $"Ёмкость второго стека: {secondStack.Capacity}");

        // демонстрируем конструктор из коллекции
        SmartStack<int> thirdStack =
            new SmartStack<int>(new int[] { 1, 2, 3 });

        Console.WriteLine("Третий стек:");

        foreach (int item in thirdStack)
            Console.Write($"{item} ");

        Console.WriteLine();

        // проверяем исключение при попытке удалить элемент из пустого стека
        try
        {
            SmartStack<int> emptyStack = new SmartStack<int>();
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        // проверяем исключение при обращении по недопустимому индексу
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