using System;

// описывает прямоугольник с координатами, размерами и вычисляемыми геометрическими характеристиками
class Rectangle
{
    // координата X прямоугольника; доступна только для чтения
    public decimal X { get; }
    // координата Y прямоугольника; доступна только для чтения
    public decimal Y { get; }
    // ширина прямоугольника
    public decimal Width { get; }
    // высота прямоугольника
    public decimal Height { get; }

    // создаёт прямоугольник и проверяет допустимость переданных значений
    // x — координата X
    // y — координата Y
    // width — ширина
    // height — высота
    // исключение ArgumentException — хотя бы одно значение отрицательное
    public Rectangle(decimal x, decimal y, decimal width, decimal height)
    {
        // запрещаем отрицательные координаты и размеры; нулевые значения разрешены
        if (x < 0 || y < 0 || width < 0 || height < 0)
        {
            throw new ArgumentException(
                "Отрицательные значения недопустимы.");
        }

        // сохраняем проверенные параметры в свойствах объекта
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    // вычисляет периметр по формуле 2 × (ширина + высота)
    public decimal Perimeter => 2 * (Width + Height);
    // вычисляет площадь как произведение ширины и высоты
    public decimal Area => Width * Height;
}

// консольный интерфейс для ввода параметров прямоугольника
class Program
{
    // метод Main: считывает параметры, создаёт объект и выводит его характеристики
    static void Main()
    {
        // перехватываем ошибки ввода и создания объекта
        try
        {
            // считываем координату X и преобразуем введённую строку в decimal
            Console.Write("Введите координату X: ");
            decimal x = decimal.Parse(Console.ReadLine());

            // считываем координату Y
            Console.Write("Введите координату Y: ");
            decimal y = decimal.Parse(Console.ReadLine());

            // считываем ширину
            Console.Write("Введите ширину: ");
            decimal width = decimal.Parse(Console.ReadLine());

            // считываем высоту
            Console.Write("Введите высоту: ");
            decimal height = decimal.Parse(Console.ReadLine());

            // создаём объект: конструктор проверит значения
            Rectangle rectangle = new Rectangle(x, y, width, height);

            // выводим свойства объекта и вычисляемые характеристики
            Console.WriteLine($"Координаты: ({rectangle.X}; {rectangle.Y})");
            Console.WriteLine($"Ширина: {rectangle.Width}");
            Console.WriteLine($"Высота: {rectangle.Height}");
            Console.WriteLine($"Периметр: {rectangle.Perimeter}");
            Console.WriteLine($"Площадь: {rectangle.Area}");
        }
        // при ошибке преобразования или проверки параметров выводим сообщение исключения
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}