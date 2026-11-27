using System;

class Rectangle
{
    public decimal X { get; }
    public decimal Y { get; }
    public decimal Width { get; }
    public decimal Height { get; }

    public Rectangle(decimal x, decimal y, decimal width, decimal height)
    {
        if (x < 0 || y < 0 || width < 0 || height < 0)
        {
            throw new ArgumentException(
                "Отрицательные значения недопустимы.");
        }

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public decimal Perimeter => 2 * (Width + Height);
    public decimal Area => Width * Height;
}

class Program
{
    static void Main()
    {
        try
        {
            Console.Write("Введите координату X: ");
            decimal x = decimal.Parse(Console.ReadLine());

            Console.Write("Введите координату Y: ");
            decimal y = decimal.Parse(Console.ReadLine());

            Console.Write("Введите ширину: ");
            decimal width = decimal.Parse(Console.ReadLine());

            Console.Write("Введите высоту: ");
            decimal height = decimal.Parse(Console.ReadLine());

            Rectangle rectangle = new Rectangle(x, y, width, height);

            Console.WriteLine($"Координаты: ({rectangle.X}; {rectangle.Y})");
            Console.WriteLine($"Ширина: {rectangle.Width}");
            Console.WriteLine($"Высота: {rectangle.Height}");
            Console.WriteLine($"Периметр: {rectangle.Perimeter}");
            Console.WriteLine($"Площадь: {rectangle.Area}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}