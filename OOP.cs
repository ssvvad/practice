using System;

/// <summary>
/// Описывает прямоугольник с координатами, размерами и вычисляемыми геометрическими характеристиками.
/// </summary>
class Rectangle
{
    /// <summary>
    /// Координата X прямоугольника. Доступна только для чтения.
    /// </summary>
    public decimal X { get; }
    /// <summary>
    /// Координата Y прямоугольника. Доступна только для чтения.
    /// </summary>
    public decimal Y { get; }
    /// <summary>
    /// Ширина прямоугольника.
    /// </summary>
    public decimal Width { get; }
    /// <summary>
    /// Высота прямоугольника.
    /// </summary>
    public decimal Height { get; }

    /// <summary>
    /// Создаёт прямоугольник и проверяет допустимость переданных значений.
    /// </summary>
    /// <param name="x">Координата X.</param>
    /// <param name="y">Координата Y.</param>
    /// <param name="width">Ширина.</param>
    /// <param name="height">Высота.</param>
    /// <exception cref="ArgumentException">Хотя бы одно значение отрицательное.</exception>
    public Rectangle(decimal x, decimal y, decimal width, decimal height)
    {
        // Запрещаем отрицательные координаты и размеры; нулевые значения разрешены.
        if (x < 0 || y < 0 || width < 0 || height < 0)
        {
            throw new ArgumentException(
                "Отрицательные значения недопустимы.");
        }

        // Сохраняем проверенные параметры в свойствах объекта.
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Вычисляет периметр по формуле 2 × (ширина + высота).
    /// </summary>
    public decimal Perimeter => 2 * (Width + Height);
    /// <summary>
    /// Вычисляет площадь как произведение ширины и высоты.
    /// </summary>
    public decimal Area => Width * Height;
}

/// <summary>
/// Консольный интерфейс для ввода параметров прямоугольника.
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа: считывает параметры, создаёт объект и выводит его характеристики.
    /// </summary>
    static void Main()
    {
        // Перехватываем ошибки ввода и создания объекта.
        try
        {
            // Считываем координату X и преобразуем введённую строку в decimal.
            Console.Write("Введите координату X: ");
            decimal x = decimal.Parse(Console.ReadLine());

            // Считываем координату Y.
            Console.Write("Введите координату Y: ");
            decimal y = decimal.Parse(Console.ReadLine());

            // Считываем ширину.
            Console.Write("Введите ширину: ");
            decimal width = decimal.Parse(Console.ReadLine());

            // Считываем высоту.
            Console.Write("Введите высоту: ");
            decimal height = decimal.Parse(Console.ReadLine());

            // Создаём объект: конструктор проверит значения.
            Rectangle rectangle = new Rectangle(x, y, width, height);

            // Выводим свойства объекта и вычисляемые характеристики.
            Console.WriteLine($"Координаты: ({rectangle.X}; {rectangle.Y})");
            Console.WriteLine($"Ширина: {rectangle.Width}");
            Console.WriteLine($"Высота: {rectangle.Height}");
            Console.WriteLine($"Периметр: {rectangle.Perimeter}");
            Console.WriteLine($"Площадь: {rectangle.Area}");
        }
        // При ошибке преобразования или проверки параметров выводим сообщение исключения.
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}